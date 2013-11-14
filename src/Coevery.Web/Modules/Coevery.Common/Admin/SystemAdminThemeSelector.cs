using System.Web.Routing;
using Coevery.Themes;
using Coevery.Themes.Services;

namespace Coevery.Common.Admin
{
    public class SystemAdminThemeSelector : IThemeSelector
    {
        private readonly ISiteThemeService _siteThemeService;

        public SystemAdminThemeSelector(ISiteThemeService siteThemeService)
        {
            _siteThemeService = siteThemeService;
        }
        public ThemeSelectorResult GetTheme(RequestContext context)
        {

            if (SystemAdminFilter.IsApplied(context))
            {
                string currentThemeName = _siteThemeService.GetCurrentThemeName();
                return new ThemeSelectorResult { Priority = 100, ThemeName = currentThemeName + "Admin" };
            }

            return null;
        }
    }
}
