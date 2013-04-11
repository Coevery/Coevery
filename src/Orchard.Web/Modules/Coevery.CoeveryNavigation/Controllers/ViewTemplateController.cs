using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Contents.Settings;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Core.Navigation.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Html;
using Orchard.UI;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Utility;
using Orchard.Utility.Extensions;

namespace Coevery.CoeveryNavigation.Controllers
{
    public class ViewTemplateController : Controller, IUpdateModel
    {
        private readonly IMenuService _menuService;
        private readonly INavigationManager _navigationManager;
        private readonly IMenuManager _menuManager;
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        public ViewTemplateController(IOrchardServices orchardServices,
            IMenuService menuService,
            IMenuManager menuManager,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            INavigationManager navigationManager) 
        {
            _menuService = menuService;
            _menuManager = menuManager;
            _navigationManager = navigationManager;
            _contentManager = contentManager;
            _transactionManager = transactionManager;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; private set; }

        public ActionResult List(NavigationManagementViewModel model, int? menuId)
        {
            //if (!Services.Authorizer.Authorize(Permissions.ManageMainMenu, T("Not allowed to manage the main menu")))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            IEnumerable<TitlePart> menus = Services.ContentManager.Query<TitlePart, TitlePartRecord>().OrderBy(x => x.Title).ForType("Menu").List();

            if (!menus.Any())
            {
                return RedirectToAction("Create", "Admin", new { area = "Contents", id = "Menu", returnUrl = Request.RawUrl });
            }

            IContent currentMenu = menuId == null
                ? menus.FirstOrDefault()
                : menus.FirstOrDefault(menu => menu.Id == menuId);

            if (currentMenu == null && menuId != null)
            { // incorrect menu id passed
                return RedirectToAction("List");
            }

            if (model == null)
            {
                model = new NavigationManagementViewModel();
            }

            if (model.MenuItemEntries == null || !model.MenuItemEntries.Any())
            {
                model.MenuItemEntries = _menuService.GetMenuParts(currentMenu.Id).Select(CreateMenuItemEntries).OrderBy(menuPartEntry => menuPartEntry.Position, new FlatPositionComparer()).ToList();
            }

            model.MenuItemDescriptors = _menuManager.GetMenuItemTypes();
            model.Menus = menus;
            model.CurrentMenu = currentMenu;

            // need action name as this action is referenced from another action
            return View(model);
        }

        [HttpPost, ActionName("List")]
        public ActionResult ListPOST(IList<MenuItemEntry> menuItemEntries, int? menuId)
        {
            //if (!Services.Authorizer.Authorize(Permissions.ManageMainMenu, T("Couldn't manage the main menu")))
            //    return new HttpUnauthorizedResult();

            if (menuItemEntries != null)
            {
                foreach (var menuItemEntry in menuItemEntries)
                {
                    MenuPart menuPart = _menuService.Get(menuItemEntry.MenuItemId);
                    menuPart.MenuPosition = menuItemEntry.Position;
                }
            }

            return RedirectToAction("List", new { menuId });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.ManageMainMenu, T("Couldn't manage the main menu")))
            //    return new HttpUnauthorizedResult();

            MenuPart menuPart = _menuService.Get(id);
            int? menuId = null;

            if (menuPart != null)
            {
                menuId = menuPart.Menu.Id;

                // get all sub-menu items from the same menu
                var menuItems = _menuService.GetMenuParts(menuPart.Menu.Id)
                    .Where(x => x.MenuPosition.StartsWith(menuPart.MenuPosition + "."))
                    .Select(x => x.As<MenuPart>())
                    .ToList();

                foreach (var menuItem in menuItems.Concat(new[] { menuPart }))
                {
                    // if the menu item is a concrete content item, don't delete it, just unreference the menu
                    if (!menuPart.ContentItem.TypeDefinition.Settings.ContainsKey("Stereotype") || menuPart.ContentItem.TypeDefinition.Settings["Stereotype"] != "MenuItem")
                    {
                        menuPart.Menu = null;
                    }
                    else
                    {
                        _menuService.Delete(menuItem);
                    }
                }

            }

            return RedirectToAction("List", new { menuId });
        }


        private MenuItemEntry CreateMenuItemEntries(MenuPart menuPart)
        {
            return new MenuItemEntry
            {
                MenuItemId = menuPart.Id,
                IsMenuItem = menuPart.Is<MenuItemPart>(),
                Text = menuPart.MenuText,
                Position = menuPart.MenuPosition,
                Url = menuPart.Is<MenuItemPart>()
                              ? menuPart.As<MenuItemPart>().Url
                              : _navigationManager.GetUrl(null, Services.ContentManager.GetItemMetadata(menuPart).DisplayRouteValues),
                ContentItem = menuPart.ContentItem,
            };
        }

        #region content edit
        public ActionResult Edit(int id)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);

            if (contentItem == null)
                return HttpNotFound();

            //if (!Services.Authorizer.Authorize(Permissions.EditContent, contentItem, T("Cannot edit content")))
            //    return new HttpUnauthorizedResult();

            dynamic model = _contentManager.BuildEditor(contentItem);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int id, string returnUrl)
        {
            return EditPOST(id, returnUrl, contentItem =>
            {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    _contentManager.Publish(contentItem);
            });
        }

        private ActionResult EditPOST(int id, string returnUrl, Action<ContentItem> conditionallyPublish)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.DraftRequired);

            if (contentItem == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.EditContent, contentItem, T("Couldn't edit content")))
                return new HttpUnauthorizedResult();

            string previousRoute = null;
            if (contentItem.Has<IAliasAspect>()
                && !string.IsNullOrWhiteSpace(returnUrl)
                && Request.IsLocalUrl(returnUrl)
                // only if the original returnUrl is the content itself
                && String.Equals(returnUrl, Url.ItemDisplayUrl(contentItem), StringComparison.OrdinalIgnoreCase)
                )
            {
                previousRoute = contentItem.As<IAliasAspect>().Path;
            }

            dynamic model = _contentManager.UpdateEditor(contentItem, this);
            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View("Edit", (object)model);
            }

            conditionallyPublish(contentItem);

            if (!string.IsNullOrWhiteSpace(returnUrl)
                && previousRoute != null
                && !String.Equals(contentItem.As<IAliasAspect>().Path, previousRoute, StringComparison.OrdinalIgnoreCase))
            {
                returnUrl = Url.ItemDisplayUrl(contentItem);
            }

            Services.Notifier.Information(string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                ? T("Your content has been saved.")
                : T("Your {0} has been saved.", contentItem.TypeDefinition.DisplayName));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion

        #region create menu item
        public ActionResult Create(string id, int subId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageMainMenu, T("Couldn't manage the main menu")))
                return new HttpUnauthorizedResult();

            // create a new temporary menu item
            var menuPart = Services.ContentManager.New<MenuPart>(id);

            if (menuPart == null)
                return HttpNotFound();

            // load the menu
            var menu = Services.ContentManager.Get(subId);

            if (menu == null)
                return HttpNotFound();

            try
            {
                // filter the content items for this specific menu
                menuPart.MenuPosition = Position.GetNext(_navigationManager.BuildMenu(menu));

                dynamic model = Services.ContentManager.BuildEditor(menuPart);

                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }
            catch (Exception exception)
            {
                Services.Notifier.Error(T("Creating menu item failed: {0}", exception.Message));
                return null;
            }
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public ActionResult CreatePost(string id, int subId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageMainMenu, T("Couldn't manage the main menu")))
                return new HttpUnauthorizedResult();

            var menuPart = Services.ContentManager.New<MenuPart>(id);

            if (menuPart == null)
                return HttpNotFound();

            // load the menu
            var menu = Services.ContentManager.Get(subId);

            if (menu == null)
                return HttpNotFound();

            var model = Services.ContentManager.UpdateEditor(menuPart, this);

            menuPart.MenuPosition = Position.GetNext(_navigationManager.BuildMenu(menu));
            menuPart.Menu = menu;

            Services.ContentManager.Create(menuPart);

            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            Services.Notifier.Information(T("Your {0} has been added.", menuPart.TypeDefinition.DisplayName));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion

        #region  UpdateModel
        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
        #endregion
    }
}