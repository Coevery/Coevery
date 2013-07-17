using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Orchard.CulturePicker {
    public class Routes : IRouteProvider {
        #region IRouteProvider Members

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (RouteDescriptor routeDescriptor in GetRoutes()) {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                //TODO (ermakovich): still not sure why we need it, but without this route Orchard can`t find controller action properly
                new RouteDescriptor {
                    Route = new Route(
                        "ChangeCulture",
                        new RouteValueDictionary {
                            {"area", "Orchard.CulturePicker"},
                            {"controller", "UserCulture"},
                            {"action", "ChangeCulture"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Orchard.CulturePicker"}
                        },
                        new MvcRouteHandler())
                }
            };
        }

        #endregion
    }
}