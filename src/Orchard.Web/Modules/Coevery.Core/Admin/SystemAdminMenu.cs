using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Coevery.Core.Admin
{
    public class SystemAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "SystemAdmin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Home"), "1", menu => menu.Url("~/Coevery"));
            
            builder.AddImageSet("Admin")
               .Add(T("Admin"), "3",
                    menu =>
                    {
                        menu.Add(T("Company Profile"), "1", item => item.Url("~/SystemAdmin"));
                        menu.Add(T("Manage Users"), "2", item => item.Url("~/SystemAdmin"));
                        menu.Add(T("Manage Perspectives"), "3", item => item.Url("~/SystemAdmin#/Metadata/Lead/Perspective/List"));
                    });
        }
    }
}