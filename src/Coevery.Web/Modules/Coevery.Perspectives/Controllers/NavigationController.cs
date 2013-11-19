using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.ContentManagement;
using Coevery.Core.Navigation.Models;
using Coevery.Core.Navigation.Services;
using Coevery.Core.Navigation.ViewModels;
using Coevery.Localization;
using Coevery.Perspectives.Services;
using Coevery.UI;
using Coevery.UI.Navigation;

namespace Coevery.Perspectives.Controllers {
    public class NavigationController : ApiController {
        private readonly IPositionManageService _positionManager;
        private readonly IMenuService _menuService;
        private readonly INavigationManager _navigationManager;

        public NavigationController(IMenuService menuService,
            ICoeveryServices coeveryServices,
            IPositionManageService positionManager,
            INavigationManager navigationManager) {
            Services = coeveryServices;
            _menuService = menuService;
            _positionManager = positionManager;
            _navigationManager = navigationManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ICoeveryServices Services { get; private set; }

        private MenuItemEntry CreateMenuItemEntries(MenuPart menuPart) {
            return new MenuItemEntry {
                MenuItemId = menuPart.Id,
                IsMenuItem = menuPart.Is<MenuItemPart>(),
                Text = menuPart.MenuText,
                Position = menuPart.MenuPosition,
                Description = menuPart.Description,
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
                        let treeInfo = _positionManager.ParseMenuPostion(menuEntry.Position,id)
                        select new {
                            Id = menuEntry.ContentItem.Id, 
                            DisplayName = menuEntry.Text, 
                            Description = menuEntry.Description,
                            Parent = treeInfo.ParentId,
                            Weight = treeInfo.Order,
                            Level = treeInfo.Level,
                            LeafOnly = treeInfo.IsLeaf
                        };

            var totalRecords = query.Count();
            return new {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }

        public object PostReorderInfo([FromBody]IEnumerable<string> positions) {
            try {
                var navigationItemIds = positions;
                var pos = 1;
                foreach (var navigationItemId in navigationItemIds) {
                    var contentItem = Services.ContentManager.Get(Convert.ToInt32(navigationItemId));
                    if (contentItem == null) {
                        throw new ArgumentNullException();
                    }
                    contentItem.As<MenuPart>().MenuPosition = pos.ToString();
                    Services.ContentManager.Publish(contentItem);
                    pos++;
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex.Message);
            }
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