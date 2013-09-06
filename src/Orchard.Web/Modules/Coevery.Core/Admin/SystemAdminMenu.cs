using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Coevery.Core.Admin {
    public class SystemAdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "SystemAdmin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(T("Home"), "1", menu => menu.Url("~/"),new []{"icon-home"});
        }
    }
}