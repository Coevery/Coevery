using System.Web;
using Orchard;

namespace Coevery.Core.ClientRoute {
    public interface IClientRouteTableManager : IDependency {
        object GetRouteTable();
    }
}