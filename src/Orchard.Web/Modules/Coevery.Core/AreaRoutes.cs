using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Mvc.Routes;

namespace Coevery.Core
{
    public class AreaRoutes : IRouteProvider
    {
        private readonly ShellBlueprint _blueprint;
        public AreaRoutes(ShellBlueprint blueprint)
        {
            _blueprint = blueprint;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {

            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }



        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var displayPathsPerArea = _blueprint.Controllers.GroupBy(
                x => x.AreaName,
                x => x.Feature.Descriptor.Extension);
            List<RouteDescriptor> coeveryRoutes = new List<RouteDescriptor>();
            var route = new RouteDescriptor
            {
                Route = new Route(
                    "Coevery",
                    new RouteValueDictionary {
                        {"area", "Coevery.Core"},
                        {"controller", "Home"},
                        {"action", "Index"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                        {"area", "Coevery.Core"}
                    },
                    new MvcRouteHandler())
            };
            coeveryRoutes.Add(route);

            coeveryRoutes.Add(new RouteDescriptor
            {
                Priority = -11,
                Route = new CoreRoute(new MvcRouteHandler())
            });

            foreach (var item in displayPathsPerArea)
            {
                var areaName = item.Key;
                var extensionDescriptor = item.Distinct().Single();
                var displayPath = extensionDescriptor.Path;
                SessionStateBehavior defaultSessionState;
                Enum.TryParse(extensionDescriptor.SessionState, true /*ignoreCase*/, out defaultSessionState);

                coeveryRoutes.Add(new RouteDescriptor
                {
                    Priority = -10,
                    SessionState = defaultSessionState,
                    Route = new Route(
                        "Coevery/" + displayPath + "/{controller}/{action}/{id}",
                        new RouteValueDictionary {
                            {"area", areaName},
                            {"controller", "home"},
                            {"action", "index"},
                            {"id", ""}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", areaName}
                        },
                        new MvcRouteHandler())
                });
            }
            return coeveryRoutes;
        }
       
    }
}