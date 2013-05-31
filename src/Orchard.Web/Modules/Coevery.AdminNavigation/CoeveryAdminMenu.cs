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
                         menu.Add(T("Leads"), "1", item => item.Url("~/Coevery/Metadata/Lead"));
                         menu.Add(T("Opportunities"), "2", item => item.Url("~/Coevery/Metadata/Opportunity"));
                         menu.Add(T("Opportunities"), "3", item => item.Url("~/Coevery/Metadata/Opportunity"));
                     });
        }
    }
}