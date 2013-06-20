using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Coevery.Metadata.Services;
using Coevery.Metadata.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Settings;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Title.Models;
using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Navigation;
using Orchard.Utility;

namespace Coevery.Metadata.Controllers
{
    public class PerspectiveViewTemplateController : Controller, IUpdateModel
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IPlacementService _placementService;
        private readonly Lazy<IEnumerable<IShellSettingsManagerEventHandler>> _settingsManagerEventHandlers;
        private readonly ShellSettings _settings;
        private readonly IContentManager _contentManager;
        private readonly INavigationManager _navigationManager;
        public PerspectiveViewTemplateController(
            IOrchardServices orchardServices,
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionManager contentDefinitionManager,
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


        public ActionResult CreatePerspective()
        {
            PerspectiveViewModel model = new PerspectiveViewModel();
            return View(model);
        }

        [HttpPost, ActionName("CreatePerspective")]
        public ActionResult CreatePerspectivePOST(PerspectiveViewModel model) 
        {
            var contentItem = _contentManager.New("Menu");
            contentItem.As<TitlePart>().Title = model.Title;
            _contentManager.Create(contentItem, VersionOptions.Draft);
            _contentManager.Publish(contentItem);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public ActionResult EditPerspective(int id)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);
            PerspectiveViewModel model = new PerspectiveViewModel();
            model.Title = contentItem.As<TitlePart>().Title;
            model.Id = contentItem.As<TitlePart>().Id;
            return View(model);
        }

        [HttpPost, ActionName("EditPerspective")]
        public ActionResult EditPerspectivePOST(int id,PerspectiveViewModel model) 
        {
            var contentItem = _contentManager.Get(id, VersionOptions.DraftRequired);
            contentItem.As<TitlePart>().Title = model.Title;
            _contentManager.Publish(contentItem);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public ActionResult EditNavigationItem(int id) {
            string subIdValue = this.ControllerContext.HttpContext.Request.QueryString["subId"];
            var perspectiveItem = _contentManager.Get(id, VersionOptions.Latest);
            int subId = 0;
            if (subIdValue != null)
            {
                subId = int.Parse(subIdValue);
            }
            
            NavigationViewModel model = new NavigationViewModel();
            model.Title = perspectiveItem.As<TitlePart>().Title;
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
             if (subId > 0) {
                var contentItem = _contentManager.Get(subId, VersionOptions.Latest);
                 model.EntityName = contentItem.As<MenuPart>().MenuText;
             }
            model.Entities = metadataTypes.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name,
                Selected = item.Name == model.EntityName
            }).ToList();
            model.NavigationId = subId;
            model.PrespectiveId = id;
           
            return View(model);
        }

         [HttpPost, ActionName("EditNavigationItem")]
        public ActionResult EditNavigationItemPOST(int perspectiveId, int navigationId, NavigationViewModel model) 
         {
             var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
             string pluralContentTypeName = pluralService.Pluralize(model.EntityName);
             if (navigationId == 0) {
                 //add
                 var menuPart = Services.ContentManager.New<MenuPart>("MenuItem");

                 // load the menu
                 var menu = Services.ContentManager.Get(perspectiveId);

                 menuPart.MenuText = pluralContentTypeName;
                 menuPart.MenuPosition = Position.GetNext(_navigationManager.BuildMenu(menu));
                 menuPart.Menu = menu;
                 menuPart.As<MenuItemPart>().Url = "~/Coevery#/" + pluralContentTypeName;
                 //menuPart.As<MenuItemPart>().FeatureId = "Coevery." + pluralContentTypeName;
                 Services.ContentManager.Create(menuPart);
                 if (!menuPart.ContentItem.Has<IPublishingControlAspect>() && !menuPart.ContentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                     _contentManager.Publish(menuPart.ContentItem);
             }
             else {
                 var contentItem = _contentManager.Get(navigationId, VersionOptions.DraftRequired);
                 contentItem.As<MenuPart>().MenuText = pluralContentTypeName;
                 contentItem.As<MenuItemPart>().Url = "~/Coevery#/" + pluralContentTypeName;
                 //contentItem.As<MenuItemPart>().FeatureId = "Coevery." + pluralContentTypeName;
                 _contentManager.Publish(contentItem);
             }
             return new HttpStatusCodeResult(HttpStatusCode.OK);
         }


        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}

