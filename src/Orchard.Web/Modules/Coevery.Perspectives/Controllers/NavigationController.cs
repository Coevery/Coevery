using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Core.Navigation.ViewModels;
using Orchard.Localization;
using Orchard.UI;
using Orchard.UI.Navigation;

namespace Coevery.Perspectives.Controllers {
    public class NavigationController : ApiController {
        private readonly IMenuService _menuService;
        private readonly INavigationManager _navigationManager;

        public NavigationController(IMenuService menuService,
            IOrchardServices orchardServices,
            INavigationManager navigationManager) {
            Services = orchardServices;
            _menuService = menuService;
            _navigationManager = navigationManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; private set; }

        private MenuItemEntry CreateMenuItemEntries(MenuPart menuPart) {
            return new MenuItemEntry {
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

        public object Get(int id, int page, int rows) {
            var menuEntitys = _menuService.GetMenuParts(id).Select(CreateMenuItemEntries)
                .OrderBy(menuPartEntry => menuPartEntry.Position, new FlatPositionComparer()).ToList();
            var query = from menuEntry in menuEntitys
                select new {Id = menuEntry.ContentItem.Id, DisplayName = menuEntry.Text,};

            var totalRecords = query.Count();
            return new {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }

        public void Delete(int id) {
            MenuPart menuPart = _menuService.Get(id);
            if (menuPart != null) {
                var menuItems = _menuService.GetMenuParts(menuPart.Menu.Id)
                    .Where(x => x.MenuPosition.StartsWith(menuPart.MenuPosition + "."))
                    .Select(x => x.As<MenuPart>())
                    .ToList();

                foreach (var menuItem in menuItems.Concat(new[] {menuPart})) {
                    if (!menuPart.ContentItem.TypeDefinition.Settings.ContainsKey("Stereotype") || menuPart.ContentItem.TypeDefinition.Settings["Stereotype"] != "MenuItem") {
                        menuPart.Menu = null;
                    }
                    else {
                        _menuService.Delete(menuItem);
                    }
                }
            }
        }
    }
}