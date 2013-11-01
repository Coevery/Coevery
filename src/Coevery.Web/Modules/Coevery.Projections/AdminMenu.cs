using Coevery.Localization;
using Coevery.UI.Navigation;

namespace Coevery.Projections {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("projector").Add(T("Queries"), "3",
                menu => menu
                    .Add(T("Queries"), "1.0",
                        qi => qi.Action("Index", "Admin", new { area = "Coevery.Projections" }).Permission(Permissions.ManageQueries).LocalNav())
                    .Add(T("Bindings"), "2.0", 
                        bi => bi.Action("Index", "Binding", new { area = "Coevery.Projections" }).Permission(Permissions.ManageQueries).LocalNav())
            );
        }
    }
}
