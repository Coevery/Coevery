using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Perspectives.Services;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Core.Navigation.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.UI;
using Orchard.UI.Navigation;
using Orchard.Utility.Extensions;

namespace Coevery.Perspectives.Controllers
{
    public class NavigationController : ApiController
    {
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IMenuService _menuService;
        private readonly INavigationManager _navigationManager;
        public NavigationController(IMenuService menuService,
            IContentManager contentManager,
            IOrchardServices orchardServices,
            INavigationManager navigationManager,
            IContentDefinitionService contentDefinitionService)
        {
            _contentManager = contentManager;
            _contentDefinitionService = contentDefinitionService;
            Services = orchardServices;
            _menuService = menuService;
            _navigationManager = navigationManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; private set; }

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

        public IEnumerable<object> Get(int id)
        {
            var menuEntitys = _menuService.GetMenuParts(id).Select(CreateMenuItemEntries)
                .OrderBy(menuPartEntry => menuPartEntry.Position, new FlatPositionComparer()).ToList();
            var query = from menuEntry in menuEntitys
                        select new { Id = menuEntry.ContentItem.Id, DisplayName = menuEntry.Text, };
            var queryList = query.ToList();
            return queryList;
        }

        public void Delete(int id)
        {
            MenuPart menuPart = _menuService.Get(id);
            if (menuPart != null)
            {
                var menuItems = _menuService.GetMenuParts(menuPart.Menu.Id)
                    .Where(x => x.MenuPosition.StartsWith(menuPart.MenuPosition + "."))
                    .Select(x => x.As<MenuPart>())
                    .ToList();

                foreach (var menuItem in menuItems.Concat(new[] { menuPart }))
                {
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
        }
    }
}