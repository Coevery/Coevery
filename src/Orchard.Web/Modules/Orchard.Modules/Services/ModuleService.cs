using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Features;
using Orchard.FileSystems.VirtualPath;
using Orchard.Localization;
using Orchard.Modules.Models;
using Orchard.UI.Notify;

namespace Orchard.Modules.Services {
    public class ModuleService : IModuleService {
        private readonly IFeatureManager _featureManager;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IExtensionManager _extensionManager;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly ICacheManager _cacheManager;
        private readonly IContentManager _contentManager;
        private readonly IMenuService _menuService;

        public ModuleService(
                IFeatureManager featureManager,
                IOrchardServices orchardServices,
                IVirtualPathProvider virtualPathProvider,
                IExtensionManager extensionManager,
                IShellDescriptorManager shellDescriptorManager,
                ICacheManager cacheManager,
                IContentManager contentManager,
                IMenuService menuService)
        {

            Services = orchardServices;

            _featureManager = featureManager;
            _virtualPathProvider = virtualPathProvider;
            _extensionManager = extensionManager;
            _shellDescriptorManager = shellDescriptorManager;
            _cacheManager = cacheManager;

            if (_featureManager.FeatureDependencyNotification == null) {
                _featureManager.FeatureDependencyNotification = GenerateWarning;
            }

            T = NullLocalizer.Instance;
            _contentManager = contentManager;
            _menuService = menuService;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        private void CreateMenuForFeature(FeatureDescriptor feature)
        {
            var menu = _menuService.GetMenu("Main Menu");
            //if Main Menu not exist,will not create menuitem for featrure.
            if (menu == null) return;
            var menuPart = _contentManager.Query<MenuItemPart>()
                                          .Where<MenuItemPartRecord>(x => x.FeatureId == feature.Id)
                                          .List().SingleOrDefault();
            ContentItem menuItem = null;
           
            var categoryMenu= _contentManager.Query<MenuPart>()
                                             .Where<MenuPartRecord>(t => t.MenuText == feature.Category)
                                             .List().FirstOrDefault();
            string menuPosition = "1";
            if (categoryMenu == null)
            {
                Services.Notifier.Warning(T(" category Menu for {0} was not found,menu was not created", feature.Name));
                return;
            }
            else
            {
                menuPosition = categoryMenu.MenuPosition + ".1";
            }

            if (menuPart == null)
            {
                menuItem = _contentManager.Create("MenuItem");
            }
            else
            {
                menuItem = menuPart.ContentItem;
            }

            menuItem.As<MenuPart>().MenuPosition = menuPosition;
            menuItem.As<MenuPart>().MenuText = feature.Name;
            menuItem.As<MenuPart>().Menu = menu.ContentItem;
            string urlTemp = "~/{0}/Home#/List";
            menuItem.As<MenuItemPart>().Url = string.Format(urlTemp, feature.Name);
            menuItem.As<MenuItemPart>().FeatureId = feature.Id;
        }

        private void RemoveMenuForFeature(FeatureDescriptor feature)
        {
            var menu = _menuService.GetMenu("Main Menu");
            //if Main Menu not exist,will not create menuitem for featrure.
            if (menu == null) return;

            var menuPart = _contentManager.Query<MenuPart>()
                                          .Join<MenuItemPartRecord>()
                                          .Where<MenuItemPartRecord>(x => x.FeatureId == feature.Id)
                                          .List().SingleOrDefault();
            if (menuPart != null)
            {
                _menuService.Delete(menuPart);
            }
        }

        /// <summary>
        /// Retrieves an enumeration of the available features together with its state (enabled / disabled).
        /// </summary>
        /// <returns>An enumeration of the available features together with its state (enabled / disabled).</returns>
        public IEnumerable<ModuleFeature> GetAvailableFeatures() {
            var enabledFeatures = _shellDescriptorManager.GetShellDescriptor().Features;
            return _extensionManager.AvailableExtensions()
                .SelectMany(m => _extensionManager.LoadFeatures(m.Features))
                .Select(f => AssembleModuleFromDescriptor(f, enabledFeatures
                    .FirstOrDefault(sf => string.Equals(sf.Name, f.Descriptor.Id, StringComparison.OrdinalIgnoreCase)) != null));
        }

        /// <summary>
        /// Enables a list of features.
        /// </summary>
        /// <param name="featureIds">The IDs for the features to be enabled.</param>
        public void EnableFeatures(IEnumerable<string> featureIds) {
            EnableFeatures(featureIds, false);
        }

        /// <summary>
        /// Enables a list of features.
        /// </summary>
        /// <param name="featureIds">The IDs for the features to be enabled.</param>
        /// <param name="force">Boolean parameter indicating if the feature should enable it's dependencies if required or fail otherwise.</param>
        public void EnableFeatures(IEnumerable<string> featureIds, bool force) {
            foreach (string featureId in _featureManager.EnableFeatures(featureIds, force)) 
            {
                var feature = _featureManager.GetAvailableFeatures().First(f => f.Id.Equals(featureId, StringComparison.OrdinalIgnoreCase));
                Services.Notifier.Information(T("{0} was enabled", feature.Name));

                //Add MenuItem
                this.CreateMenuForFeature(feature);
            }
        }

        /// <summary>
        /// Disables a list of features.
        /// </summary>
        /// <param name="featureIds">The IDs for the features to be disabled.</param>
        public void DisableFeatures(IEnumerable<string> featureIds) {
            DisableFeatures(featureIds, false);
        }

        /// <summary>
        /// Disables a list of features.
        /// </summary>
        /// <param name="featureIds">The IDs for the features to be disabled.</param>
        /// <param name="force">Boolean parameter indicating if the feature should disable the features which depend on it if required or fail otherwise.</param>
        public void DisableFeatures(IEnumerable<string> featureIds, bool force) {
            foreach (string featureId in _featureManager.DisableFeatures(featureIds, force)) {
                var feature = _featureManager.GetAvailableFeatures().Where(f => f.Id == featureId).First();

                this.RemoveMenuForFeature(feature);
                Services.Notifier.Information(T("{0} was disabled", feature.Name));
            }
        }

        /// <summary>
        /// Determines if a module was recently installed by using the project's last written time.
        /// </summary>
        /// <param name="extensionDescriptor">The extension descriptor.</param>
        public bool IsRecentlyInstalled(ExtensionDescriptor extensionDescriptor) {
            DateTime lastWrittenUtc = _cacheManager.Get(extensionDescriptor, descriptor => {
                string projectFile = GetManifestPath(extensionDescriptor);
                if (!string.IsNullOrEmpty(projectFile)) {
                    // If project file was modified less than 24 hours ago, the module was recently deployed
                    return _virtualPathProvider.GetFileLastWriteTimeUtc(projectFile);
                }

                return DateTime.UtcNow;
            });

            return DateTime.UtcNow.Subtract(lastWrittenUtc) < new TimeSpan(1, 0, 0, 0);
        }

        /// <summary>
        /// Retrieves the full path of the manifest file for a module's extension descriptor.
        /// </summary>
        /// <param name="extensionDescriptor">The module's extension descriptor.</param>
        /// <returns>The full path to the module's manifest file.</returns>
        private string GetManifestPath(ExtensionDescriptor extensionDescriptor) {
            string projectPath = _virtualPathProvider.Combine(extensionDescriptor.Location, extensionDescriptor.Id, "module.txt");

            if (!_virtualPathProvider.FileExists(projectPath)) {
                return null;
            }

            return projectPath;
        }

        private static ModuleFeature AssembleModuleFromDescriptor(Feature feature, bool isEnabled) {
            return new ModuleFeature {
                                         Descriptor = feature.Descriptor,
                                         IsEnabled = isEnabled
                                     };
        }

        private void GenerateWarning(string messageFormat, string featureName, IEnumerable<string> featuresInQuestion) {
            if (featuresInQuestion.Count() < 1)
                return;

            Services.Notifier.Warning(T(
                messageFormat,
                featureName,
                featuresInQuestion.Count() > 1
                    ? string.Join("",
                                  featuresInQuestion.Select(
                                      (fn, i) =>
                                      T(i == featuresInQuestion.Count() - 1
                                            ? "{0}"
                                            : (i == featuresInQuestion.Count() - 2
                                                   ? "{0} and "
                                                   : "{0}, "), fn).ToString()).ToArray())
                    : featuresInQuestion.First()));
        }
    }
}