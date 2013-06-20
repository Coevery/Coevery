using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Models;
using Coevery.Core.Services;
using Newtonsoft.Json.Linq;
using Orchard;

namespace Coevery.Projections.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {

        public void GetClientRoutes(ICollection<ClientRoute> clientRoutes)
        {
            foreach (var clientRoute in GetClientRoutes())
                clientRoutes.Add(clientRoute);
        }

        public IEnumerable<ClientRoute> GetClientRoutes() {
            return BaseClientRoute.GetClientRoutes("Projection", "Projections");
        }
    }
}