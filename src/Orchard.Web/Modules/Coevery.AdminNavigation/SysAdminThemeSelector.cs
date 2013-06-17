using System.Web.Routing;
using Orchard.Themes;

namespace Coevery.AdminNavigation {
    public class SysAdminThemeSelector : IThemeSelector {
        public ThemeSelectorResult GetTheme(RequestContext context) {
            if (SysAdminFilter.IsApplied(context)) {
                return new ThemeSelectorResult {Priority = 100, ThemeName = "MooncakeAdmin"};
            }
            return null;
        }
    }
}
