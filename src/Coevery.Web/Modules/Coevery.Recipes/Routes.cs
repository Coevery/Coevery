using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Coevery.Mvc.Routes;

namespace Coevery.Recipes {
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                             new RouteDescriptor {   Priority = 5,
                                                     Route = new Route(
                                                         "Recipes/Status/{executionId}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Coevery.Recipes"},
                                                                                      {"controller", "Recipes"},
                                                                                      {"action", "RecipeExecutionStatus"}
                                                         },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Coevery.Recipes"}
                                                         },
                                                         new MvcRouteHandler())
                             }
                         };
        }
    }
}