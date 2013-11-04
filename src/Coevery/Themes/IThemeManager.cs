using System.Web.Routing;
using Coevery.Environment.Extensions.Models;

namespace Coevery.Themes {
    public interface IThemeManager : IDependency {
        ExtensionDescriptor GetRequestTheme(RequestContext requestContext);
    }
}
