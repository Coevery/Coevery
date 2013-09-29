using System.Collections.Generic;
using System.Web;
using Orchard;

namespace Coevery.Core.ClientRoute {
    public interface IClientRouteTableManager : IDependency {
        IEnumerable<ClientRouteDescriptor> GetRouteTable(bool isFrontEnd);
    }
}