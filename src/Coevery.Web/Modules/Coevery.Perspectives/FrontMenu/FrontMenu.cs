using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using Coevery.Common.Extensions;
using Coevery.Common.Models;
using Coevery.Core.Navigation.Models;
using Coevery.Core.Navigation.Services;
using Coevery.Core.Navigation.ViewModels;
using Coevery.Core.Title.Models;
using Coevery.Localization;
using Coevery.UI;
using Coevery.UI.Navigation;
using Coevery.ContentManagement;
using System.Linq;

namespace Coevery.Perspectives.FrontMenu
{
    public class FrontMenu : INavigationProvider {
        private readonly IMenuService _menuService;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;

        public FrontMenu(
            ICoeveryServices coeveryServices, 
            IContentDefinitionExtension contentDefinitionExtension,
            IMenuService menuService) {
            _contentDefinitionExtension = contentDefinitionExtension;
            Services = coeveryServices;
            _menuService = menuService;
        }

        public Localizer T { get; set; }

        public string MenuName {
            get { return "FrontMenu"; }
        }

        public ICoeveryServices Services { get; set; }

        public void GetNavigation(NavigationBuilder builder) {
            var menus = Services.ContentManager.Query<TitlePart, TitlePartRecord>().OrderBy(x => x.Title).ForType("Menu").List().ToList();
            int firstMenuIndex = 0;
            menus.ForEach(c => {
                builder.AddImageSet(c.Title)
                    .Add(T(c.Title), (++firstMenuIndex).ToString(),
                        menu => {
                            string url = string.Format("#/{0}", c.Id);
                            menu.Url(url);
                            menu.Content(c);
                            menu.IdHint(c.Id.ToString(CultureInfo.InvariantCulture));
                            int menuIdex = 0;
                            var subMenus = _menuService.GetMenuParts(c.Id)
                                .OrderBy(menuPartEntry => menuPartEntry.MenuPosition, new FlatPositionComparer())
                                .ToList();
                            foreach (var subMenu in subMenus) {
                                var subMenuCotent = subMenu;
                                var menuItemEntity = CreateMenuItemEntries(subMenu, url);
                                var moduleMenuItem = subMenu.As<ModuleMenuItemPart>();
                                var position = (++menuIdex).ToString(CultureInfo.InvariantCulture);
                                if (moduleMenuItem != null) {
                                    menu.Add(T(menuItemEntity.Text), position, item =>
                                        item.Url(menuItemEntity.Url).Content(subMenuCotent).IdHint(subMenuCotent.Id.ToString(CultureInfo.InvariantCulture)),
                                        new List<string>() {moduleMenuItem.IconClass});
                                }
                                else {
                                    menu.Add(T(menuItemEntity.Text), position, item =>
                                        item.Url(menuItemEntity.Url).Content(subMenuCotent).IdHint(subMenuCotent.Id.ToString(CultureInfo.InvariantCulture)));
                                }
                            }
                        });
            });
        }

        private MenuItemEntry CreateMenuItemEntries(MenuPart menuPart, string parentUrl) {
            string urlFormat = parentUrl + "/{0}";
            string url;

            if (menuPart.Is<ModuleMenuItemPart>())
            {
                var urlName = _contentDefinitionExtension.GetEntityNames(
                    menuPart.As<ModuleMenuItemPart>().ContentTypeDefinitionRecord.Name).CollectionName;
                url = string.Format(urlFormat, urlName);
            }
            else
            {
                url = string.Format(urlFormat, menuPart.MenuText);
            }

            return new MenuItemEntry {
                MenuItemId = menuPart.Id,
                IsMenuItem = menuPart.Is<MenuItemPart>(),
                Text = menuPart.MenuText,
                Position = menuPart.MenuPosition,
                Url = url,
                ContentItem = menuPart.ContentItem,
            };
        }
    }
}