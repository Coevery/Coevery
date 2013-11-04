using System.Collections.Generic;

namespace Coevery.Mvc.Routes {
    public interface IRoutePublisher : IDependency {
        void Publish(IEnumerable<RouteDescriptor> routes);
    }
}