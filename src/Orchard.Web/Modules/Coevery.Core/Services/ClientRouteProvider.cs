using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Models;
using Orchard;

namespace Coevery.Core.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {

        public void GetClientRoutes(ICollection<ClientRoute> clientRoutes)
        {
            foreach (var clientRoute in GetClientRoutes())
                clientRoutes.Add(clientRoute);
        }

        public IEnumerable<ClientRoute> GetClientRoutes()
        {
            yield return new
          ClientRoute
                {
                    StateName = "List",
                    ClientRouteInfo = new ClientRouteInfo
                    {
                        z = true,
                        Definition = new Definition
                        {
                            Url = string.Empty,
                            TemplateUrl = string.Empty,
                            Controller = string.Empty,
                            Dependencies = new string[] { }
                        },
                        Children = new Dictionary<string, ClientRouteInfo>
                        {
                            {"Detail",new ClientRouteInfo() {
                                z = true,
                                Definition = new Definition
                                {
                                    Url = string.Empty,
                                    TemplateUrl = string.Empty,
                                    Controller = string.Empty,
                                    Dependencies = new string[] { }
                                }
                            }}
                        }
                    }

                };
        }
    }
}