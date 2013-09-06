using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Coevery.Perspectives.Services
{
    public class SystemAdminMenu : INavigationProvider {

        public Localizer T { get; set; }
        public string MenuName { get { return "SystemAdmin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("Admin")
                .Add(T("Admin"), "3",
                    menu => menu.Add(T("Manage Perspectives"), "1", item => item.Url("~/SystemAdmin#/Perspectives")),
                    new[] { "icon-cogs" });
        }
    }
}