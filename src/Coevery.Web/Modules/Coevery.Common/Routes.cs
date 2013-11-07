using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Coevery.Environment.ShellBuilders.Models;
using Coevery.Mvc.Routes;

namespace Coevery.Common {
    public class Routes : IRouteProvider {
        private readonly ShellBlueprint _blueprint;

        public Routes(ShellBlueprint blueprint) {
            _blueprint = blueprint;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes()) {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            var displayPathsPerArea = _blueprint.Controllers.GroupBy(
                x => x.AreaName,
                x => x.Feature.Descriptor.Extension);

            yield return new RouteDescriptor {
                Priority = 100,
                Route = new Route(
                    "",
                    new RouteValueDictionary {
                        {"area", "Coevery.Common"},
                        {"controller", "Home"},
                        {"action", "Index"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                        {"area", "Coevery.Common"}
                    },
                    new MvcRouteHandler())
            };


            yield return new RouteDescriptor {
                Route = new Route(
                    "SystemAdmin",
                    new RouteValueDictionary {
                        {"area", "Coevery.Common"},
                        {"controller", "SystemAdmin"},
                        {"action", "Index"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                        {"area", "Coevery.Common"}
                    },
                    new MvcRouteHandler())
            };
            foreach (var item in displayPathsPerArea) {
                var areaName = item.Key;
                var extensionDescriptor = item.Distinct().Single();
                var displayPath = extensionDescriptor.Path;
                SessionStateBehavior defaultSessionState;
                Enum.TryParse(extensionDescriptor.SessionState, true /*ignoreCase*/, out defaultSessionState);

                yield return new RouteDescriptor {
                    Priority = -10,
                    SessionState = defaultSessionState,
                    Route = new Route(
                        displayPath + "/{controller}/{action}/{id}",
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
                };

                yield return new RouteDescriptor {
                    Priority = -10,
                    SessionState = defaultSessionState,
                    Route = new Route(
                        "SystemAdmin/" + displayPath + "/{action}/{id}",
                        new RouteValueDictionary {
                            {"area", areaName},
                            {"controller", "SystemAdmin"},
                            {"action", "index"},
                            {"id", ""}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", areaName}
                        },
                        new MvcRouteHandler())
                };
            }

            yield return new RouteDescriptor {
                Priority = -11,
                Route = new DefaultRoute()
            };
        }
    }
}