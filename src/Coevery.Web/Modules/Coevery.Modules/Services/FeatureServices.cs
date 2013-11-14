using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Common.Extensions;
using Coevery.Data.Migration;
using Coevery.Environment.Descriptor.Models;
using Coevery.Environment.Extensions.Models;
using Coevery.Environment.Features;
using Coevery.Localization;
using Coevery.Modules.Models;

namespace Coevery.Modules.Services {
    public class FeatureServices : IFeatureServices {
        private readonly IDataMigrationManager _dataMigrationManager;
        private readonly IModuleService _moduleService;
        private readonly IFeatureManager _featureManager;
        private readonly ShellDescriptor _shellDescriptor;

        public FeatureServices(IContentDefinitionExtension contentDefinitionService,
            IDataMigrationManager dataMigrationManager,
            IModuleService moduleService,
            IFeatureManager featureManager,
            ShellDescriptor shellDescriptor) {
            _dataMigrationManager = dataMigrationManager;
            _moduleService = moduleService;
            _featureManager = featureManager;
            _shellDescriptor = shellDescriptor;
            T = NullLocalizer.Instance;
        }

        private bool GetState(FeaturesBulkAction bulkAction, IList<string> featureIds, FeatureDescriptor featureDescriptor) {
            if (bulkAction == FeaturesBulkAction.Enable) {
                return true;
            }
            else if(bulkAction == FeaturesBulkAction.Disable) {
                return false;
            }
            return _shellDescriptor.Features.Any(sf => sf.Name == featureDescriptor.Id);
        }

        public Localizer T { get; set; }

        public IEnumerable<FeatureCategory> GeFeatureCategories(FeaturesBulkAction bulkAction, IList<string> featureIds)
        {
            var featuresThatNeedUpdate = _dataMigrationManager.GetFeaturesThatNeedUpdate();
            IEnumerable<ModuleFeature> featureslist = _featureManager.GetAvailableFeatures()
                .Where(f => !DefaultExtensionTypes.IsTheme(f.Extension.ExtensionType))
                .Select(f => new ModuleFeature
                {
                    Descriptor = f,
                    IsEnabled = GetState(bulkAction, featureIds,f),
                    IsRecentlyInstalled = _moduleService.IsRecentlyInstalled(f.Extension),
                    NeedsUpdate = featuresThatNeedUpdate.Contains(f.Id)
                });
            
            var featureentrylist = new List<FeatureCategory>();

            var featuresGroup = featureslist.Where(f=>(bulkAction == FeaturesBulkAction.None ? true : featureIds.Contains(f.Descriptor.Id))).OrderBy(f => f.Descriptor.Category, new DoghouseComparer("Core")).GroupBy(f => f.Descriptor.Category).ToList();
            foreach (var featureGroup in featuresGroup)
            {
                var categoryName = LocalizedString.TextOrDefault(featureGroup.First().Descriptor.Category, T("Uncategorized"));
                var featureCategory = new FeatureCategory();
                featureCategory.CategoryName = categoryName;
                var featureentries = new List<FeatureEntry>();
                var features = featureGroup.OrderBy(f => f.Descriptor.Name);
                foreach (var feature in features) {
                    var featureentry = new FeatureEntry();
                    featureentry.FeatureId = feature.Descriptor.Id;
                    featureentry.FeatureName = string.IsNullOrEmpty(feature.Descriptor.Name) ? feature.Descriptor.Id : feature.Descriptor.Name;
                    featureentry.FeatureDescription = feature.Descriptor.Description;
                    featureentry.FeatureState = feature.IsEnabled ? "enable" : "disable";
                    featureentry.TitleStyle = feature.IsEnabled ? "background-image: -webkit-linear-gradient(top, #46815B, #3F8A4A); border:none;cursor:pointer;"
                        : "background-image:-webkit-linear-gradient(top, #858E97, #858E97); border:none;cursor:pointer;";
                    featureentry.IsRecentlyInstalled = feature.IsRecentlyInstalled;
                    featureentry.NeedsUpdate = feature.NeedsUpdate;
                    featureentry.Dependencies = (from d in feature.Descriptor.Dependencies
                                                 select (from f in featureslist where f.Descriptor.Id.Equals(d, StringComparison.OrdinalIgnoreCase) select f).SingleOrDefault()).Where(f => f != null).OrderBy(f => f.Descriptor.Name);
                    var missingDependencies = feature.Descriptor.Dependencies
                        .Where(d => !featureslist.Any(f => f.Descriptor.Id.Equals(d, StringComparison.OrdinalIgnoreCase)));
                    var showDisable = categoryName.ToString() != "Core";
                    var showEnable = !missingDependencies.Any() && feature.Descriptor.Id != "Coevery.Setup";
                    var featureAction = new FeatureAction();
                    featureAction.IsShow = false;
                    if (showDisable && feature.IsEnabled) {
                        featureAction.IsShow = true;
                        featureAction.Action = Enum.GetName(typeof(FeaturesBulkAction),FeaturesBulkAction.Disable);
                        featureAction.Iconclass = "icol-delete";
                        featureAction.Content = T("Disable");
                        featureAction.Force = true;
                    }
                    else if (showEnable && !feature.IsEnabled)
                    {
                        featureAction.IsShow = true;
                        featureAction.Action = Enum.GetName(typeof(FeaturesBulkAction),FeaturesBulkAction.Enable);
                        featureAction.Iconclass = "icol-accept";
                        featureAction.Content = T("Enable");
                        featureAction.Force = true;
                    }
                    if (featureentry.NeedsUpdate)
                    {
                        featureAction.IsShow = true;
                        featureAction.Action = Enum.GetName(typeof(FeaturesBulkAction),FeaturesBulkAction.Update);
                        featureAction.Iconclass = "icol-accept";
                        featureAction.Content = T("Update");
                        featureAction.Force = false;
                    }

                    featureentry.FeatureAction = featureAction;
                    featureentries.Add(featureentry);
                }
                featureCategory.Features = featureentries;
                featureentrylist.Add(featureCategory);
            }
            return featureentrylist;
        }
    }
}