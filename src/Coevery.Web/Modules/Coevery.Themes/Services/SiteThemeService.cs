using Coevery.Caching;
using Coevery.ContentManagement;
using Coevery.Environment.Extensions;
using Coevery.Environment.Extensions.Models;
using Coevery.Themes.Models;

namespace Coevery.Themes.Services {
    public interface ISiteThemeService : IDependency {
        ExtensionDescriptor GetSiteTheme();
        void SetSiteTheme(string themeName);
        string GetCurrentThemeName();
    }

    public class SiteThemeService : ISiteThemeService {
        public const string CurrentThemeSignal = "SiteCurrentTheme";

        private readonly IExtensionManager _extensionManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly ICoeveryServices _coeveryServices;

        public SiteThemeService(
            ICoeveryServices coeveryServices,
            IExtensionManager extensionManager,
            ICacheManager cacheManager,
            ISignals signals) {

            _coeveryServices = coeveryServices;
            _extensionManager = extensionManager;
            _cacheManager = cacheManager;
            _signals = signals;
        }

        public ExtensionDescriptor GetSiteTheme() {
            string currentThemeName = GetCurrentThemeName();
            return string.IsNullOrEmpty(currentThemeName) ? null : _extensionManager.GetExtension(GetCurrentThemeName());
        }

        public void SetSiteTheme(string themeName) {
            var site = _coeveryServices.WorkContext.CurrentSite;
            site.As<ThemeSiteSettingsPart>().CurrentThemeName = themeName;

            _signals.Trigger(CurrentThemeSignal);
        }

        public string GetCurrentThemeName() {
            return _cacheManager.Get("CurrentThemeName", ctx => {
                ctx.Monitor(_signals.When(CurrentThemeSignal));
                return _coeveryServices.WorkContext.CurrentSite.As<ThemeSiteSettingsPart>().CurrentThemeName;
            });
        }
    }
}
