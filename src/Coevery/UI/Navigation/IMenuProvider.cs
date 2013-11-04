using Coevery.ContentManagement;

namespace Coevery.UI.Navigation {
    public interface IMenuProvider : IDependency {
        void GetMenu(IContent menu, NavigationBuilder builder);
    }
}