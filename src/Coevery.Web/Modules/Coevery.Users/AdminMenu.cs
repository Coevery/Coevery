using Coevery.Localization;
using Coevery.Security;
using Coevery.UI.Navigation;

namespace Coevery.Users {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("users")
                .Add(T("Users"), "11",
                    menu => menu.Action("Index", "Admin", new { area = "Coevery.Users" }).Permission(StandardPermissions.SiteOwner)
                        .Add(T("Users"), "1.0", item => item.Action("Index", "Admin", new { area = "Coevery.Users" })
                            .LocalNav().Permission(StandardPermissions.SiteOwner)));
        }
    }
}
