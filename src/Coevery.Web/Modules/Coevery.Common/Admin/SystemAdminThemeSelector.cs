using System.Web.Routing;
using Coevery.Themes;

namespace Coevery.Common.Admin
{
    public class SystemAdminThemeSelector : IThemeSelector
    {
        public ThemeSelectorResult GetTheme(RequestContext context)
        {
            if (SystemAdminFilter.IsApplied(context))
            {
                return new ThemeSelectorResult { Priority = 100, ThemeName = "MooncakeAdmin" };
            }

            return null;
        }
    }
}
