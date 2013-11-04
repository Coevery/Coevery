using Coevery.Environment.Extensions;
using Coevery.Localization;
using Coevery.Security;
using Coevery.UI.Navigation;

namespace Coevery.Alias
{
    [CoeveryFeature("Coevery.Alias.UI")]
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .Add(T("Aliases"), "4", item => item.Action("Index", "Admin", new { area = "Coevery.Alias" }).Permission(StandardPermissions.SiteOwner));
        }
    }
}