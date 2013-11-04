using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Security;
using Coevery.Settings;
using Coevery.UI.Navigation;

namespace Coevery.Core.Settings {
    public class AdminMenu : INavigationProvider {
        private readonly ISiteService _siteService;

        public AdminMenu(ISiteService siteService, ICoeveryServices coeveryServices) {
            _siteService = siteService;
            Services = coeveryServices;
        }

        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }
        public ICoeveryServices Services { get; private set; }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("settings")
                .Add(T("Settings"), "99",
                    menu => menu.Add(T("General"), "0", item => item.Action("Index", "Admin", new { area = "Settings", groupInfoId = "Index" })
                        .Permission(StandardPermissions.SiteOwner)), new [] {"collapsed"});

            var site = _siteService.GetSiteSettings();
            if (site == null)
                return;

            foreach (var groupInfo in Services.ContentManager.GetEditorGroupInfos(site.ContentItem)) {
                GroupInfo info = groupInfo;
                builder.Add(T("Settings"),
                    menu => menu.Add(info.Name, info.Position, item => item.Action("Index", "Admin", new { area = "Settings", groupInfoId = info.Id })
                        .Permission(StandardPermissions.SiteOwner)));
            }
        }
    }
}
