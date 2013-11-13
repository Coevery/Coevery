using Coevery.Localization;
using Coevery.UI.Navigation;

namespace Coevery.Roles.Services {
    public class SystemAdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public string MenuName {
            get { return "SystemAdmin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .Add(T("Admin"),
                    menu => menu.Add(T("Roles"), "3", item => item.Url("~/SystemAdmin#/Roles")),
                    new[] {"icon-cogs"});
        }
    }
}