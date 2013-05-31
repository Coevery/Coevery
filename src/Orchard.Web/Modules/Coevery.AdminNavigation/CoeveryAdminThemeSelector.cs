using System.Web.Routing;
using Orchard.Themes;

namespace Coevery.AdminNavigation
{
    public class CoeveryAdminThemeSelector : IThemeSelector
    {
        public ThemeSelectorResult GetTheme(RequestContext context)
        {
            if (CoeveryAdminFilter.IsApplied(context))
            {
                return new ThemeSelectorResult { Priority = 100, ThemeName = "MooncakeAdmin" };
            }

            return null;
        }
    }
}
