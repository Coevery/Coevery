using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Models;
using Orchard;

namespace Coevery.Core.Services
{
    public interface IClientRouteProvider:IDependency {
        void GetClientRoutes(ICollection<ClientRoute> clientRoutes);
        IEnumerable<ClientRoute> GetClientRoutes();
    }
}