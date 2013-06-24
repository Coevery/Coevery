using System.Web.Routing;
using Orchard.Themes;

namespace Coevery.Core.Admin {
    public class SystemAdminThemeSelector : IThemeSelector {
        public ThemeSelectorResult GetTheme(RequestContext context) {
            if (SystemAdminFilter.IsApplied(context)) {
                return new ThemeSelectorResult { Priority = 100, ThemeName = "MooncakeAdmin" };
            }
            return null;
        }
    }
}
