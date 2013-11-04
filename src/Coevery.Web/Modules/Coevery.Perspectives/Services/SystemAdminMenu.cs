using Coevery.ContentManagement.MetaData;
using Coevery.Localization;
using Coevery.UI.Navigation;

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