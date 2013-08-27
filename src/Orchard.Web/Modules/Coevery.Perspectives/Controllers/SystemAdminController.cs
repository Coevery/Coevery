using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Coevery.Core;
using Coevery.Core.Models;
using Coevery.Perspectives.Services;
using Coevery.Perspectives.ViewModels;
using Orchard;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Contents.Settings;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.Utility;
using Orchard.ContentManagement;
using IContentManager = Orchard.ContentManagement.IContentManager;
using VersionOptions = Orchard.ContentManagement.VersionOptions;

namespace Coevery.Perspectives.Controllers
{
    public class SystemAdminController : Controller
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IRepository<ContentTypeDefinitionRecord> _contentTypeDefinitionRepository;
        private readonly IPlacementService _placementService;
        private readonly Lazy<IEnumerable<IShellSettingsManagerEventHandler>> _settingsManagerEventHandlers;
        private readonly ShellSettings _settings;
        private readonly IContentManager _contentManager;
        private readonly INavigationManager _navigationManager;
        public SystemAdminController(
            IOrchardServices orchardServices,
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionManager contentDefinitionManager,
            IRepository<ContentTypeDefinitionRecord> contentTypeDefinitionRepository,
            IPlacementService placementService,
            Lazy<IEnumerable<IShellSettingsManagerEventHandler>> settingsManagerEventHandlers,
             IContentManager contentManager,
            ShellSettings settings,
            INavigationManager navigationManager
            )
        {
            Services = orchardServices;
            _contentDefinitionService = contentDefinitionService;
            _contentDefinitionManager = contentDefinitionManager;
            _contentTypeDefinitionRepository = contentTypeDefinitionRepository;
            _placementService = placementService;
            _settingsManagerEventHandlers = settingsManagerEventHandlers;
            _settings = settings;
            _contentManager = contentManager;
            _navigationManager = navigationManager;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult List(string id)
        {
            return View();
        }


        public ActionResult Create()
        {
            PerspectiveViewModel model = new PerspectiveViewModel();
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public CustomJsonResult CreatePOST(PerspectiveViewModel model) {
            var result = new CustomJsonResult();
            result.Try(() => {
                           var contentItem = _contentManager.New("Menu");
                           contentItem.As<TitlePart>().Title = model.Title;
                           _contentManager.Create(contentItem, VersionOptions.Draft);
                           _contentManager.Publish(contentItem);
                           result.Value = contentItem.Id;
                       });

            return result;
        }


        public ActionResult Edit(int id)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);
            PerspectiveViewModel model = new PerspectiveViewModel();
            model.Title = contentItem.As<TitlePart>().Title;
            model.Id = contentItem.As<TitlePart>().Id;
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        public CustomJsonResult EditPOST(int id, PerspectiveViewModel model)
        {
            var result = new CustomJsonResult();
            result.Try(() => {
                           var contentItem = _contentManager.Get(id, VersionOptions.DraftRequired);
                           contentItem.As<TitlePart>().Title = model.Title;
                           _contentManager.Publish(contentItem);
                           result.Value = contentItem.Id;
                       });

            return result;
        }

        public ActionResult Detail(int id)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);
            PerspectiveViewModel model = new PerspectiveViewModel();
            model.Title = contentItem.As<TitlePart>().Title;
            model.Id = contentItem.As<TitlePart>().Id;
            return View(model);
        }

        public ActionResult EditNavigationItem(int id)
        {

            NavigationViewModel model = new NavigationViewModel();

            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            model.EntityName = contentItem.As<MenuPart>().MenuText;
            model.EntityName = pluralService.Singularize(model.EntityName);
            var menuId = contentItem.As<MenuPart>().Record.MenuId;
            var perspectiveItem = _contentManager.Get(menuId, VersionOptions.Latest);
            model.Title = perspectiveItem.As<TitlePart>().Title;
            model.IconClass = contentItem.As<ModuleMenuItemPart>().IconClass;
            model.Entities = metadataTypes.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name,
                Selected = item.Name == model.EntityName
            }).ToList();
            model.NavigationId = id;
            model.PrespectiveId = menuId;
            return View(model);
        }

        [HttpPost, ActionName("EditNavigationItem")]
        public ActionResult EditNavigationItemPOST(int perspectiveId, int navigationId, NavigationViewModel model)
        {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            string pluralContentTypeName = pluralService.Pluralize(model.EntityName);

            var contentItem = _contentManager.Get(navigationId, VersionOptions.DraftRequired);
            contentItem.As<MenuPart>().MenuText = pluralContentTypeName;
            //contentItem.As<MenuItemPart>().Url = "~/Coevery#/" + pluralContentTypeName;
            contentItem.As<ModuleMenuItemPart>().ContentTypeDefinitionRecord = _contentTypeDefinitionRepository.Table.FirstOrDefault(x => x.Name == model.EntityName);
            contentItem.As<ModuleMenuItemPart>().IconClass = model.IconClass;
            //contentItem.As<MenuItemPart>().FeatureId = "Coevery." + pluralContentTypeName;
            _contentManager.Publish(contentItem);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult CreateNavigationItem(int id)
        {
            NavigationViewModel model = new NavigationViewModel();
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
            var perspectiveItem = _contentManager.Get(id, VersionOptions.Latest);
            model.Title = perspectiveItem.As<TitlePart>().Title;
            model.Entities = metadataTypes.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name,
                Selected = item.Name == model.EntityName
            }).ToList();
            model.NavigationId = 0;
            model.PrespectiveId = id;
            return View(model);
        }

        [HttpPost, ActionName("CreateNavigationItem")]
        public ActionResult CreateNavigationItemPOST(int perspectiveId, int navigationId, NavigationViewModel model)
        {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            string pluralContentTypeName = pluralService.Pluralize(model.EntityName);

            //add
            var moduleMenuPart = Services.ContentManager.New<MenuPart>("ModuleMenuItem");

            // load the menu
            var menu = Services.ContentManager.Get(perspectiveId);

            moduleMenuPart.MenuText = pluralContentTypeName;
            moduleMenuPart.MenuPosition = Position.GetNext(_navigationManager.BuildMenu(menu));
            moduleMenuPart.Menu = menu;
            //menuPart.As<MenuItemPart>().Url = "~/Coevery#/" + pluralContentTypeName;
            moduleMenuPart.As<ModuleMenuItemPart>().ContentTypeDefinitionRecord = _contentTypeDefinitionRepository.Table.FirstOrDefault(x => x.Name == model.EntityName);
            moduleMenuPart.As<ModuleMenuItemPart>().IconClass = model.IconClass;
            //menuPart.As<MenuItemPart>().FeatureId = "Coevery." + pluralContentTypeName;
            Services.ContentManager.Create(moduleMenuPart);
            if (!moduleMenuPart.ContentItem.Has<IPublishingControlAspect>() && !moduleMenuPart.ContentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                _contentManager.Publish(moduleMenuPart.ContentItem);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}