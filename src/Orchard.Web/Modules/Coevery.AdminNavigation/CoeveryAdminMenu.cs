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
            builder.AddImageSet("Home")
                .Add(T("Home"), "1",
                     menu =>
                     {
                         menu.Add(T("Home"), "1", item => item.Url("~/Coevery"));
                         menu.Add(T("Home"), "1", item => item.Url("~/Coevery"));
                     });
            builder.AddImageSet("Entities")
                .Add(T("Entities"), "2",
                     menu =>
                     {
                         menu.Add(T("Leads"), "1", item => item.Url("~/CoeveryAdmin#/Metadata/Lead"));
                         menu.Add(T("Opportunities"), "2", item => item.Url("~/CoeveryAdmin#/Metadata/Opportunity"));
                         menu.Add(T("Invoices"), "3", item => item.Url("~/CoeveryAdmin#/Metadata/Invoice"));
                     });
            builder.AddImageSet("Admin")
               .Add(T("Admin"), "3",
                    menu =>
                    {
                        menu.Add(T("Company Profile"), "1", item => item.Url("#"));
                        menu.Add(T("Manager Users"), "2", item => item.Url("#"));
                    });
        }
    }
}