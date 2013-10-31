using System.Collections.Generic;
using System.Web;
using Coevery;

namespace Coevery.Common.ClientRoute {
    public interface IClientRouteTableManager : IDependency {
        IEnumerable<ClientRouteDescriptor> GetRouteTable(bool isFrontEnd);
    }
}