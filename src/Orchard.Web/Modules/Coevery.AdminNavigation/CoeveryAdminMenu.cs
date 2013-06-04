using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Coevery.AdminNavigation
{
    public class CoeveryAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "CoeveryAdmin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Home"), "1", menu => menu.Url("~/Coevery"));
            builder.AddImageSet("Entities")
                .Add(T("Entities"), "2",
                     menu =>
                     {
                         menu.Add(T("All Entities"), "1", item => item.Url("~/CoeveryAdmin#/Metadata"));
                         menu.Add(T("Leads"), "2", item => item.Url("~/CoeveryAdmin#/Metadata/Lead"));
                         menu.Add(T("Opportunities"), "3", item => item.Url("~/CoeveryAdmin#/Metadata/Opportunity"));
                         menu.Add(T("Invoices"), "4", item => item.Url("~/CoeveryAdmin#/Metadata/Invoice"));
                     });
            builder.AddImageSet("Admin")
               .Add(T("Admin"), "3",
                    menu =>
                    {
                        menu.Add(T("Company Profile"), "1", item => item.Url("~/CoeveryAdmin"));
                        menu.Add(T("Manager Users"), "2", item => item.Url("~/CoeveryAdmin"));
                        menu.Add(T("Manager Perspective"), "3", item => item.Url("~/CoeveryAdmin#/Metadata/Lead/Perspective/List"));
                    });
        }
    }
}