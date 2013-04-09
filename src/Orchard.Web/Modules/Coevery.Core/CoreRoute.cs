using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Extensions;

namespace Coevery.Core {

    public class CoreRoute : RouteBase, IRouteWithArea {

        private readonly RouteBase _route;

        public string Area { get; private set; }

        public CoreRoute(RouteBase route) {
            _route = route;
            Area = route.GetAreaName();
        }

        public override RouteData GetRouteData(HttpContextBase httpContext) {
            var routeData = _route.GetRouteData(httpContext);
            if (routeData == null)
                return null;
            routeData.Values["area"] = "Coevery.Core";
            routeData.DataTokens["area"] = "Coevery.Core";
            routeData.Values["controller"] = "ContentViewTemplate";
            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
            return _route.GetVirtualPath(requestContext, values);
        }
    }
}
