using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Mvc.Extensions;

namespace Orchard.Mvc.Routes {

    public class CoreRoute : RouteBase, IRouteWithArea {

        public string Area { get; private set; }
        private readonly RouteBase _route;
        private readonly ShellSettings _shellSettings;
        private readonly IRunningShellTable _runningShellTable;
        private readonly UrlPrefix _urlPrefix;
        private readonly IRouteHandler _routeHandler;
        public CoreRoute(IRouteHandler routeHandler)
        {
            _routeHandler = routeHandler;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext) {
            string url = httpContext.Request.AppRelativeCurrentExecutionFilePath;
            string filterKey = "~/Coevery/";
            if (url.StartsWith(filterKey) && url.Length > filterKey.Length) {
                string[] urlArrs = url.Split(new char[] {'/'});
                var routeData = RouteTable.Routes.GetRouteData(httpContext);
                routeData.Values["area"] = "Coevery.Core";
                routeData.DataTokens["area"] = "Coevery.Core";
                routeData.Values["controller"] = "ContentViewTemplate";
                routeData.Values["id"] = urlArrs[urlArrs.Length - 1];
                routeData.Values["action"] = urlArrs[urlArrs.Length - 2];
                return routeData;
            }

            return null;
        }

        


        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
            //var match = _aliasMap.Locate(routeValues);
            //if (match != null)
            //{
            //    // Build any "spare" route values onto the Alias (so we correctly support any additional query parameters)
            //    var sb = new StringBuilder(match.Item2);
            //    var extra = 0;
            //    foreach (var routeValue in routeValues)
            //    {
            //        // Ignore any we already have
            //        if (match.Item1.ContainsKey(routeValue.Key))
            //        {
            //            continue;
            //        }

            //        // Add a query string fragment
            //        sb.Append((extra++ == 0) ? '?' : '&');
            //        sb.Append(Uri.EscapeDataString(routeValue.Key));
            //        sb.Append('=');
            //        sb.Append(Uri.EscapeDataString(Convert.ToString(routeValue.Value, CultureInfo.InvariantCulture)));
            //    }
            //    // Construct data
            //    var data = new VirtualPathData(this, sb.ToString());
            //    // Set the Area for this route
            //    data.DataTokens["area"] = Area;
            //    return data;
            //}

            return null;
        }
     
    }
}
