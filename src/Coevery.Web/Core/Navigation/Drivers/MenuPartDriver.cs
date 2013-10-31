using JetBrains.Annotations;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.Core.Navigation.Models;
using Coevery.Core.Navigation.Services;
using Coevery.Core.Navigation.ViewModels;
using Coevery.Localization;
using Coevery.Security;
using Coevery.UI.Navigation;
using Coevery.Utility;

namespace Coevery.Core.Navigation.Drivers {
    [UsedImplicitly]
    public class MenuPartDriver : ContentPartDriver<MenuPart> {
        private readonly IAuthorizationService _authorizationService;
        private readonly INavigationManager _navigationManager;
        private readonly ICoeveryServices _coeveryServices;
        private readonly IMenuService _menuService;

        public MenuPartDriver(
            IAuthorizationService authorizationService, 
            INavigationManager navigationManager, 
            ICoeveryServices coeveryServices,
            IMenuService menuService) {
            _authorizationService = authorizationService;
            _navigationManager = navigationManager;
            _coeveryServices = coeveryServices;
            _menuService = menuService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix {
            get {
                return "MenuPart";
            }
        }

        protected override DriverResult Editor(MenuPart part, dynamic shapeHelper) {
            if (!_authorizationService.TryCheckAccess(Permissions.ManageMainMenu, _coeveryServices.WorkContext.CurrentUser, part))
                return null;

            return ContentShape("Parts_Navigation_Menu_Edit", () => {
                var model = new MenuPartViewModel {
                    CurrentMenuId = part.Menu == null ? -1 : part.Menu.Id,
                    ContentItem = part.ContentItem,
                    Menus = _menuService.GetMenus(),
                    OnMenu = part.Menu != null,
                    MenuText = part.MenuText
                };

                return shapeHelper.EditorTemplate(TemplateName: "Parts.Navigation.Menu.Edit", Model: model, Prefix: Prefix);
            });
        }

        protected override DriverResult Editor(MenuPart part, IUpdateModel updater, dynamic shapeHelper) {
            if (!_authorizationService.TryCheckAccess(Permissions.ManageMainMenu, _coeveryServices.WorkContext.CurrentUser, part))
                return null;

            var model = new MenuPartViewModel();

            if(updater.TryUpdateModel(model, Prefix, null, null)) {
                var menu = model.OnMenu ? _coeveryServices.ContentManager.Get(model.CurrentMenuId) : null;

                part.MenuText = model.MenuText;
                part.Menu = menu;

                if (string.IsNullOrEmpty(part.MenuPosition) && menu != null) {
                    part.MenuPosition = Position.GetNext(_navigationManager.BuildMenu(menu));

                    if (string.IsNullOrEmpty(part.MenuText)) {
                        updater.AddModelError("MenuText", T("The MenuText field is required"));
                    }
                }
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(MenuPart part, ContentManagement.Handlers.ImportContentContext context) {
            var menuText = context.Attribute(part.PartDefinition.Name, "MenuText");
            if (menuText != null) {
                part.MenuText = menuText;
            }

            var position = context.Attribute(part.PartDefinition.Name, "MenuPosition");
            if (position != null) {
                part.MenuPosition = position;
            }

            var menuIdentity = context.Attribute(part.PartDefinition.Name, "Menu");
            if (menuIdentity != null) {
                var menu = context.GetItemFromSession(menuIdentity);
                if (menu != null) {
                    part.Menu = menu;
                }
            }
        }

        protected override void Exporting(MenuPart part, ContentManagement.Handlers.ExportContentContext context) {
            // is it on a menu ?
            if(part.Menu == null) {
                return;
            }

            var menu = _coeveryServices.ContentManager.Get(part.Menu.Id);
            var menuIdentity = _coeveryServices.ContentManager.GetItemMetadata(menu).Identity;
            context.Element(part.PartDefinition.Name).SetAttributeValue("Menu", menuIdentity);

            context.Element(part.PartDefinition.Name).SetAttributeValue("MenuText", part.MenuText);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MenuPosition", part.MenuPosition);
        }
    }
}