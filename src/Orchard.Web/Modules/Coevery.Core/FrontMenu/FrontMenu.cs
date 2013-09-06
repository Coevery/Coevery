using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using Coevery.Core.Models;
using Orchard;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;
using Orchard.Core.Navigation.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.UI;
using Orchard.UI.Navigation;
using Orchard.ContentManagement;
using System.Linq;
namespace Coevery.Core.FrontMenu
{
    public class FrontMenu : INavigationProvider {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public Localizer T { get; set; }
        public string MenuName { get { return "FrontMenu"; } }
        public IOrchardServices Services { get; set; }
        private readonly IMenuService _menuService;
        //private readonly INavigationManager _navigationManager;
        public FrontMenu(IOrchardServices orchardServices, IMenuService menuService,
           
            IContentDefinitionManager contentDefinitionManager)
        {
            Services = orchardServices;
            _menuService = menuService;
            //_navigationManager = navigationManager;
            _contentDefinitionManager = contentDefinitionManager;
        }

        private MenuItemEntry CreateMenuItemEntries(MenuPart menuPart)
        {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            const string urlTemp = "~/Coevery#/{0}";
            string pluralContentTypeName = pluralService.Pluralize(menuPart.MenuText);
            string url = string.Format(urlTemp, pluralContentTypeName);

            return new MenuItemEntry
            {

                MenuItemId = menuPart.Id,
                IsMenuItem = menuPart.Is<MenuItemPart>(),
                Text = menuPart.MenuText,
                Position = menuPart.MenuPosition,
                Url = url,
                ContentItem = menuPart.ContentItem,
            };
        }

        public void GetNavigation(NavigationBuilder builder) {

            var menus = Services.ContentManager.Query<TitlePart, TitlePartRecord>().OrderBy(x => x.Title).ForType("Menu").List().ToList();
            int firstMenuIndex = 0;
            menus.ForEach(c => {
                builder.AddImageSet(c.Title)
                   .Add(T(c.Title), (++firstMenuIndex).ToString(),
                        menu =>
                        {
                            int menuIdex = 0;
                            var subMenus = _menuService.GetMenuParts(c.Id).
                                OrderBy(menuPartEntry => menuPartEntry.MenuPosition, 
                                new FlatPositionComparer()).ToList();
                            foreach (var subMenu in subMenus) {

                                //var moduleMenuItem = c.As<ModuleMenuItemPart>();
                                //if(moduleMenuItem!=null) {
                                
                                //}

                                var menuItemEntity = CreateMenuItemEntries(subMenu);
                                var moduleMenuItem = subMenu.As<ModuleMenuItemPart>();
                                if (moduleMenuItem != null) {
                                    menu.Add(T(menuItemEntity.Text), (++menuIdex).ToString(), item => item.Url(menuItemEntity.Url), new List<string>() { moduleMenuItem.IconClass});
                                }
                                else
                                {
                                    menu.Add(T(menuItemEntity.Text), (++menuIdex).ToString(), item => item.Url(menuItemEntity.Url));   
                                }
                            }
                        });
            });
        }
    }
}