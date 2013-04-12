using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Extensions;

namespace Coevery.Core {

    public class CoreRoute : RouteBase {

        private readonly RouteBase _route;

        public CoreRoute()
        {
            _route = new Route("Coevery/{area}/{controller}/{action}/{id}", new MvcRouteHandler());
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
