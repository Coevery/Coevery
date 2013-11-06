using System.Collections.Generic;

namespace Coevery.Mvc.ClientRoute
{
    public interface IClientRouteTableManager : IDependency {
        IEnumerable<ClientRouteDescriptor> GetRouteTable(bool isFrontEnd);
    }
}