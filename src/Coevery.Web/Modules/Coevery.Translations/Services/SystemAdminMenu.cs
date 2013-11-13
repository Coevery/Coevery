

using Coevery.Localization;
using Coevery.UI.Navigation;
namespace Coevery.Translations.Services
{
    public class SystemAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName
        {
            get { return "SystemAdmin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("Translation")
                .Add(T("Translation"), "4",
                    menu =>
                    {
                        menu.Add(T("Translate"), "1", item => item.Url("~/SystemAdmin#/Translations/Index"));
                        menu.Add(T("Import"), "2", item => item.Url("~/SystemAdmin#/Translations/Import"));
                        //menu.Add(T("Export"), "3", item => item.Url("~/SystemAdmin#/Translations/Export"));
                        menu.Add(T("Search"), "4", item => item.Url("~/SystemAdmin#/Translations/Search"));
                    },
                    new[] { "icon-book" });
        }
    }
}