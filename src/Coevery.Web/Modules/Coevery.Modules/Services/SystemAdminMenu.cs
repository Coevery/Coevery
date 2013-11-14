using Coevery.Localization;
using Coevery.UI.Navigation;

namespace Coevery.Modules.Services
{
    public class SystemAdminMenu : INavigationProvider {

        public Localizer T { get; set; }
        public string MenuName { get { return "SystemAdmin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("Modules")
                .Add(T("Modules"), "4",
                    menu => {
                        menu.Add(T("Features"), "1", item => item.Url("~/SystemAdmin#/Modules/Features"));
                        menu.Add(T("Installed"), "2", item => item.Url("~/SystemAdmin#/Modules/Installed"));
                        menu.Add(T("Recipes"), "3", item => item.Url("~/SystemAdmin#/Modules/Recipes"));
                    },
                    new[] { "icon-shopping-cart" });
        }
    }
}