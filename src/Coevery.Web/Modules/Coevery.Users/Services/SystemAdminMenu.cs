using Coevery.Localization;
using Coevery.UI.Navigation;

namespace Coevery.Users.Services {
    public class SystemAdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public string MenuName {
            get { return "SystemAdmin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .Add(T("Admin"),
                    menu => menu.Add(T("Users"), "2", item => item.Url("~/SystemAdmin#/Users")),
                    new[] {"icon-cogs"});
        }
    }
}