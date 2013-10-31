using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.Core.Navigation.Models;

namespace Coevery.Core.Navigation.Services {
    public interface IMenuService : IDependency {
        IEnumerable<MenuPart> Get();
        IEnumerable<MenuPart> GetMenuParts(int menuId);
        MenuPart Get(int id);
        IContent GetMenu(int menuId);
        IContent GetMenu(string name);
        IEnumerable<ContentItem> GetMenus();
        IContent Create(string name);
        void Delete(MenuPart menuPart);
    }
}