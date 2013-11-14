using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Coevery.Data.Migration;
using Coevery.DisplayManagement;
using Coevery.Environment.Descriptor.Models;
using Coevery.Environment.Extensions;
using Coevery.Environment.Extensions.Models;
using Coevery.Environment.Features;
using Coevery.Localization;
using Coevery.Logging;
using Coevery.Modules.Events;
using Coevery.Modules.Models;
using Coevery.Modules.Services;
using Coevery.Modules.ViewModels;
using Coevery.Mvc;
using Coevery.Recipes.Models;
using Coevery.Recipes.Services;
using Coevery.Reports.Services;
using Coevery.Security;
using Coevery.UI.Navigation;
using Coevery.UI.Notify;
using Newtonsoft.Json;

namespace Coevery.Modules.Controllers {
    public class SystemAdminController : Controller {
        private readonly IExtensionDisplayEventHandler _extensionDisplayEventHandler;
        private readonly IModuleService _moduleService;
        private readonly IDataMigrationManager _dataMigrationManager;
        private readonly IReportsCoordinator _reportsCoordinator;
        private readonly IExtensionManager _extensionManager;
        private readonly IFeatureManager _featureManager;
        private readonly IRecipeHarvester _recipeHarvester;
        private readonly IRecipeManager _recipeManager;
        private readonly ShellDescriptor _shellDescriptor;

        private readonly IFeatureServices _featureServices; 



        public SystemAdminController(
            IEnumerable<IExtensionDisplayEventHandler> extensionDisplayEventHandlers,
            ICoeveryServices services,
            IModuleService moduleService,
            IDataMigrationManager dataMigrationManager,
            IReportsCoordinator reportsCoordinator,
            IExtensionManager extensionManager,
            IFeatureManager featureManager,
            IRecipeHarvester recipeHarvester,
            IRecipeManager recipeManager,
            ShellDescriptor shellDescriptor,
            IShapeFactory shapeFactory,
            IFeatureServices featureServices)
        {
            Services = services;
            _extensionDisplayEventHandler = extensionDisplayEventHandlers.FirstOrDefault();
            _moduleService = moduleService;
            _dataMigrationManager = dataMigrationManager;
            _reportsCoordinator = reportsCoordinator;
            _extensionManager = extensionManager;
            _featureManager = featureManager;
            _recipeHarvester = recipeHarvester;
            _recipeManager = recipeManager;
            _shellDescriptor = shellDescriptor;
            Shape = shapeFactory;
            _featureServices = featureServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ICoeveryServices Services { get; set; }
        public ILogger Logger { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Index(ModulesIndexOptions options, PagerParameters pagerParameters) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not allowed to manage modules")))
                return new HttpUnauthorizedResult();

            Pager pager = new Pager(Services.WorkContext.CurrentSite, pagerParameters);

            IEnumerable<ModuleEntry> modules = _extensionManager.AvailableExtensions()
                .Where(extensionDescriptor => DefaultExtensionTypes.IsModule(extensionDescriptor.ExtensionType) &&
                                              (string.IsNullOrEmpty(options.SearchText) || extensionDescriptor.Name.ToLowerInvariant().Contains(options.SearchText.ToLowerInvariant())))
                .OrderBy(extensionDescriptor => extensionDescriptor.Name)
                .Select(extensionDescriptor => new ModuleEntry { Descriptor = extensionDescriptor });

            int totalItemCount = modules.Count();

            if (pager.PageSize != 0) {
                modules = modules.Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize);
            }

            modules = modules.ToList();
            foreach (ModuleEntry moduleEntry in modules) {
                moduleEntry.IsRecentlyInstalled = _moduleService.IsRecentlyInstalled(moduleEntry.Descriptor);

                if (_extensionDisplayEventHandler != null) {
                    foreach (string notification in _extensionDisplayEventHandler.Displaying(moduleEntry.Descriptor, ControllerContext.RequestContext)) {
                        moduleEntry.Notifications.Add(notification);
                    }
                }
            }

            return View(new ModulesIndexViewModel {
                Modules = modules,
                InstallModules = _featureManager.GetEnabledFeatures().FirstOrDefault(f => f.Id == "PackagingServices") != null,
                Options = options,
                Pager = Shape.Pager(pager).TotalItemCount(totalItemCount)
            });
        }

        public ActionResult Recipes() {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not allowed to manage modules")))
                return new HttpUnauthorizedResult();

            IEnumerable<ModuleEntry> modules = _extensionManager.AvailableExtensions()
                .Where(extensionDescriptor => DefaultExtensionTypes.IsModule(extensionDescriptor.ExtensionType))
                .OrderBy(extensionDescriptor => extensionDescriptor.Name)
                .Select(extensionDescriptor => new ModuleEntry { Descriptor = extensionDescriptor });

            var viewModel = new RecipesViewModel();

            if (_recipeHarvester != null) {
                viewModel.Modules = modules.Select(x => new ModuleRecipesViewModel {
                    Module = x,
                    Recipes = _recipeHarvester.HarvestRecipes(x.Descriptor.Id).ToList()
                })
                .Where(x => x.Recipes.Any());
            }

            return View(viewModel);

        }

        [HttpPost, ActionName("Recipes")]
        public ActionResult RecipesPOST(string moduleId, string name) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not allowed to manage modules")))
                return new HttpUnauthorizedResult();

            ModuleEntry module = _extensionManager.AvailableExtensions()
                .Where(extensionDescriptor => extensionDescriptor.Id == moduleId)
                .Select(extensionDescriptor => new ModuleEntry { Descriptor = extensionDescriptor }).FirstOrDefault();

            if (module == null) {
                return HttpNotFound();
            }

            Recipe recipe = _recipeHarvester.HarvestRecipes(module.Descriptor.Id).FirstOrDefault(x => x.Name == name);

            if (recipe == null) {
                return HttpNotFound();
            }

            try {
                _recipeManager.Execute(recipe);
            }
            catch(Exception e) {
                Logger.Error(e, "Error while executing recipe {0} in {1}", moduleId, name);
                Services.Notifier.Error(T("Recipes contains {0} unsupported module installation steps.", recipe.Name));
            }

            Services.Notifier.Information(T("The recipe {0} was executed successfully.", recipe.Name));
            
            return RedirectToAction("Recipes");

        }
        public ActionResult Features() {
            if (!Services.Authorizer.Authorize(Permissions.ManageFeatures, T("Not allowed to manage features")))
                return new HttpUnauthorizedResult();
            return View();
        }

        [HttpPost, ActionName("Features")]
        [FormValueRequired("submit.BulkExecute")]
        public object FeaturesPOST(FeaturesBulkAction bulkAction, IList<string> featureIds, bool? force) {

            if (!Services.Authorizer.Authorize(Permissions.ManageFeatures, T("Not allowed to manage features")))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, T("Not allowed to manage features").Text);

            if (featureIds == null || !featureIds.Any()) {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, T("Please select one or more features").Text);
            }

            if (ModelState.IsValid) {
                var availableFeatures = _moduleService.GetAvailableFeatures().ToList();
                var selectedFeatures = availableFeatures.Where(x => featureIds.Contains(x.Descriptor.Id)).ToList();
                var enabledFeatures = availableFeatures.Where(x => x.IsEnabled && featureIds.Contains(x.Descriptor.Id)).Select(x => x.Descriptor.Id).ToList();
                var disabledFeatures = availableFeatures.Where(x => !x.IsEnabled && featureIds.Contains(x.Descriptor.Id)).Select(x => x.Descriptor.Id).ToList();

                switch (bulkAction) {
                    case FeaturesBulkAction.None:
                        break;
                    case FeaturesBulkAction.Enable:
                        _moduleService.EnableFeatures(disabledFeatures, force == true);
                        break;
                    case FeaturesBulkAction.Disable:
                        _moduleService.DisableFeatures(enabledFeatures, force == true);
                        break;
                    case FeaturesBulkAction.Update:
                        foreach (var feature in selectedFeatures.Where(x => x.NeedsUpdate)) {
                            var id = feature.Descriptor.Id;
                            try {
                                _reportsCoordinator.Register("Data Migration", "Upgrade " + id, "Coevery installation");
                                _dataMigrationManager.Update(id);
                            }
                            catch (Exception exception) {
                                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed, T("An error occured while updating the feature {0}: {1}", id, exception.Message).Text);
                            }
                        }
                        break;
                    default:
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, T("argument not found").Text);
                }
                var featureentrylist = _featureServices.GeFeatureCategories(bulkAction,featureIds.ToList());
                var json = JsonConvert.SerializeObject(featureentrylist, Formatting.None, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                return json;
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, T("novalid").Text);
        }
    }
}