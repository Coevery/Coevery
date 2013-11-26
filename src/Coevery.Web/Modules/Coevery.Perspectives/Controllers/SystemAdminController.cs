using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Coevery.Common.Extensions;
using Coevery.Common.Models;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Core.Common.ViewModels;
using Coevery.Perspectives.Models;
using Coevery.Perspectives.Services;
using Coevery.Perspectives.ViewModels;
using Coevery.ContentManagement.Aspects;
using Coevery.Core.Contents.Settings;
using Coevery.Core.Navigation.Models;
using Coevery.Core.Settings.Metadata.Records;
using Coevery.Core.Title.Models;
using Coevery.Data;
using Coevery.Localization;
using Coevery.Logging;
using Coevery.UI.Navigation;
using Coevery.Utility;
using Coevery.ContentManagement;

namespace Coevery.Perspectives.Controllers {
    public class SystemAdminController : Controller {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IRepository<ContentTypeDefinitionRecord> _contentTypeDefinitionRepository;
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;
        private readonly INavigationManager _navigationManager;
        private readonly IPositionManageService _positionManager;

        public SystemAdminController(
            ICoeveryServices coeveryServices,
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionExtension contentDefinitionExtension,
            IRepository<ContentTypeDefinitionRecord> contentTypeDefinitionRepository,
            IPositionManageService positionManager,
            IContentManager contentManager,
            INavigationManager navigationManager) {
            Services = coeveryServices;
            _contentDefinitionExtension = contentDefinitionExtension;
            _contentDefinitionService = contentDefinitionService;
            _contentTypeDefinitionRepository = contentTypeDefinitionRepository;
            _contentManager = contentManager;
            _navigationManager = navigationManager;
            _positionManager = positionManager;
            T = NullLocalizer.Instance;
        }

        public ICoeveryServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult List(string id) {
            return View(new GridRowSortSettingViewModel {
                Url = "api/Perspectives/Perspective/PostReorderInfo",
                Method = "Post",
                Handler = string.Empty
            });
        }


        public ActionResult Create() {
            PerspectiveViewModel model = new PerspectiveViewModel();
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(PerspectiveViewModel model) {
            var contentItem = _contentManager.New("Menu");
            var currPos = 0;
            var lastPos = _contentManager.Query<PerspectivePart, PerspectivePartRecord>().OrderByDescending(x => x.Position).ForType("Menu").List().FirstOrDefault();
            if (lastPos != null) {
                currPos = lastPos.Position;
                currPos++;
            }
            contentItem.As<PerspectivePart>().Title = model.Title;
            contentItem.As<PerspectivePart>().Description = model.Description;
            contentItem.As<PerspectivePart>().Position = currPos;
            contentItem.As<PerspectivePart>().CurrentLevel = 0;
            _contentManager.Create(contentItem, VersionOptions.Draft);
            _contentManager.Publish(contentItem);
            return Json(new { id = contentItem.Id });
        }


        public ActionResult Edit(int id) {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);
            PerspectiveViewModel model = new PerspectiveViewModel();
            model.Id = contentItem.As<PerspectivePart>().Id;
            model.Title = contentItem.As<PerspectivePart>().Title;
            model.Description = contentItem.As<PerspectivePart>().Description;
            model.Position = contentItem.As<PerspectivePart>().Position;
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(PerspectiveViewModel model) {
            var contentItem = _contentManager.Get(model.Id, VersionOptions.DraftRequired);
            contentItem.As<PerspectivePart>().Title = model.Title;
            contentItem.As<PerspectivePart>().Description = model.Description;
            contentItem.As<PerspectivePart>().Position = model.Position;
            _contentManager.Publish(contentItem);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Detail(int id) {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);
            var model = new PerspectiveViewModel {
                Id = contentItem.As<PerspectivePart>().Id,
                Title = contentItem.As<PerspectivePart>().Title,
                Description = contentItem.As<PerspectivePart>().Description,
                Position = contentItem.As<PerspectivePart>().Position,
                NavigationTypeList = new List<ContentTypeDefinition>()
            };
            foreach (var type in PerspectiveViewModel.NavigationTypes) {
                var temp = _contentManager.GetContentTypeDefinitions().FirstOrDefault(definition => definition.Name == type);
                if (temp != null) {
                    model.NavigationTypeList.Add(temp);
                }
            }
            return View(model);
        }

        public ActionResult EditNavigationItem(int id, string type) {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);
            var menuId = contentItem.As<MenuPart>().Record.MenuId;
            var perspectiveItem = _contentManager.Get(menuId, VersionOptions.Latest);
            var model = new NavigationViewModel {
                NavigationId = id,
                PrespectiveId = menuId,
                Type = type,
                Description = contentItem.As<MenuPart>().Description,
                Title = perspectiveItem.As<PerspectivePart>().Title
            };
            switch (type) {
                case "MenuItem":
                    model.Url = contentItem.As<MenuItemPart>().Url;
                    model.EntityName = contentItem.As<MenuPart>().MenuText;
                    break;
                default:
                    var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
                    model.EntityName = _contentDefinitionExtension.GetEntityNameFromCollectionName(
                        contentItem.As<MenuPart>().MenuText, true);
                    model.IconClass = contentItem.As<ModuleMenuItemPart>().IconClass;
                    model.Entities = metadataTypes.Select(item => new SelectListItem {
                        Text = item.DisplayName,
                        Value = item.Name,
                        Selected = item.Name == model.EntityName
                    }).ToList();
                    break;
            }
            return View(model);
        }

        [HttpPost, ActionName("EditNavigationItem")]
        public ActionResult EditNavigationItemPOST(int perspectiveId, int navigationId, string type, NavigationViewModel model) {
            var menu = _contentManager.Get<PerspectivePart>(perspectiveId);
            var contentItem = _contentManager.Get(navigationId, VersionOptions.DraftRequired);

            //Used as the module id of front end
            switch (type) {
                case "MenuItem":
                    Uri myUri;
                    if (!Uri.TryCreate(model.Url, UriKind.RelativeOrAbsolute, out myUri)) {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Content(T("Invalid url!").ToString());
                    }
                    contentItem.As<MenuPart>().MenuText = model.EntityName;
                    contentItem.As<MenuItemPart>().Url = model.Url;
                    break;
                default:
                    if (string.IsNullOrWhiteSpace(model.IconClass)) {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Content(T("Icon is required.").ToString());
                    }
                    var pluralContentTypeName = _contentDefinitionExtension.GetEntityNames(model.EntityName);

                    contentItem.As<MenuPart>().MenuText = pluralContentTypeName.CollectionDisplayName;
                    contentItem.As<ModuleMenuItemPart>().ContentTypeDefinitionRecord = _contentTypeDefinitionRepository.Table.FirstOrDefault(x => x.Name == model.EntityName);
                    contentItem.As<ModuleMenuItemPart>().IconClass = model.IconClass;
                    break;
            }
            contentItem.As<MenuPart>().Description = model.Description;
            _contentManager.Publish(contentItem);

            menu.CurrentLevel = _positionManager.GetPositionLevel(contentItem.As<MenuPart>().MenuPosition);
            _contentManager.Publish(menu.ContentItem);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult CreateNavigationItem(int id, string type) {

            var perspectiveItem = _contentManager.Get(id, VersionOptions.Latest);
            var model = new NavigationViewModel {
                Type = type,
                Title = perspectiveItem.As<PerspectivePart>().Title,
                PrespectiveId = id,
            };
            model.Type = type;
            switch (type) {
                case "MenuItem":
                    break;
                default:
                    var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
                    if (metadataTypes == null)
                        break;
                    model.Entities = metadataTypes.Select(item => new SelectListItem {
                        Text = item.DisplayName,
                        Value = item.Name,
                        Selected = item.Name == model.EntityName
                    }).ToList();
                    model.NavigationId = 0;
                    break;
            }
            return View(model);
        }

        [HttpPost, ActionName("CreateNavigationItem")]
        public ActionResult CreateNavigationItemPOST(int perspectiveId, int navigationId, string type, NavigationViewModel model) {
            //add
            var moduleMenuPart = Services.ContentManager.New<MenuPart>(type);

            // load the menu
            var menu = Services.ContentManager.Get<PerspectivePart>(perspectiveId);

            switch (type) {
                case "MenuItem":
                    Uri myUri;
                    if (!Uri.TryCreate(model.Url, UriKind.RelativeOrAbsolute, out myUri)) {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Content(T("Invalid url!").ToString());
                    }
                    moduleMenuPart.MenuText = model.EntityName;
                    moduleMenuPart.As<MenuItemPart>().Url = model.Url;
                    break;
                default:
                    if (string.IsNullOrWhiteSpace(model.IconClass)) {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Content(T("Icon is required.").ToString());
                    }
                    var pluralContentTypeName = _contentDefinitionExtension.GetEntityNames(model.EntityName);

                    moduleMenuPart.MenuText = pluralContentTypeName.CollectionDisplayName;
                    moduleMenuPart.As<ModuleMenuItemPart>().ContentTypeDefinitionRecord = _contentTypeDefinitionRepository.Table.FirstOrDefault(x => x.Name == model.EntityName);
                    moduleMenuPart.As<ModuleMenuItemPart>().IconClass = model.IconClass;
                    break;
            }
            moduleMenuPart.MenuPosition = Position.GetNext(_navigationManager.BuildMenu(menu));
            moduleMenuPart.Menu = menu;
            moduleMenuPart.Description = model.Description;

            Services.ContentManager.Create(moduleMenuPart);
            if (!moduleMenuPart.ContentItem.Has<IPublishingControlAspect>() && !moduleMenuPart.ContentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable) {
                _contentManager.Publish(moduleMenuPart.ContentItem);
            }
            menu.CurrentLevel = _positionManager.GetPositionLevel(moduleMenuPart.MenuPosition);
            _contentManager.Publish(menu.ContentItem);
            return Json(new { id = moduleMenuPart.ContentItem.Id, type = model.Type });
        }

    }
}