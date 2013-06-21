using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Models;
using Coevery.Core.Services;
using Newtonsoft.Json.Linq;
using Orchard;

namespace Coevery.Perspectives.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {

        public void GetClientRoutes(ICollection<ClientRoute> clientRoutes)
        {
            foreach (var clientRoute in GetClientRoutes())
                clientRoutes.Add(clientRoute);
        }

        public IEnumerable<ClientRoute> GetClientRoutes() {
            var baseClientRoute = BaseClientRoute.GetClientRoutes("Perspective", "Perspectives");
            foreach (var clientRoute in baseClientRoute) {
                yield return clientRoute;
            }
            baseClientRoute.First(c => c.StateName == "PerspectiveDetail").ClientRouteInfo.children = new Dictionary<string, ClientRouteInfo> {
                {
                    "EditNavigationItem", new ClientRouteInfo {
                        definition = new Definition {
                            url = "/Navigation/{NId:[0-9a-zA-Z]+}",
                            templateUrl = new JRaw("function(params) { return 'SystemAdmin/Perspectives/EditNavigationItem/' + params.NId;}"),
                            controller = "NavigationItemDetailCtrl",
                            dependencies = new[] {"Modules/Coevery.Perspectives/Scripts/controllers/navigationitemdetailcontroller"}
                        }
                    }
                }
            };
            yield return new ClientRoute
            {
                StateName = "EditNavigationItem",
                ClientRouteInfo = new ClientRouteInfo
                {
                    definition = new Definition
                    {
                        url = "/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/{NId:[0-9a-zA-Z]+}",
                        templateUrl = new JRaw("function(params) { return 'SystemAdmin/Perspectives/EditNavigationItem/' + params.NId;}"),
                        controller = "NavigationItemDetailCtrl",
                        dependencies = new[] { "Modules/Coevery.Perspectives/Scripts/controllers/navigationitemdetailcontroller" }
                    }
                }
            };

            yield return new ClientRoute
            {
                StateName = "CreateNavigationItem",
                ClientRouteInfo = new ClientRouteInfo
                {
                    definition = new Definition
                    {
                        url = "/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/Create",
                        templateUrl = new JRaw("function(params) { return 'SystemAdmin/Perspectives/CreateNavigationItem/' + params.Id;}"),
                        controller = "NavigationItemCreateCtrl",
                        dependencies = new[] { "Modules/Coevery.Perspectives/Scripts/controllers/navigationitemcreatecontroller" }
                    }
                }
            };
        }
    }
}