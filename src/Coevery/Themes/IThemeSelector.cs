using System.Web.Routing;

namespace Coevery.Themes {
    public class ThemeSelectorResult {
        public int Priority { get; set; }
        public string ThemeName { get; set; }
    }

    public interface IThemeSelector : IDependency {
        ThemeSelectorResult GetTheme(RequestContext context);
    }

}