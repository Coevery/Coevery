using System.Collections.Generic;
using Coevery.Mvc.Routes;

namespace Coevery.WebApi.Routes {
    public interface IHttpRouteProvider : IDependency {
        IEnumerable<RouteDescriptor> GetRoutes();
        void GetRoutes(ICollection<RouteDescriptor> routes);
    }
}
