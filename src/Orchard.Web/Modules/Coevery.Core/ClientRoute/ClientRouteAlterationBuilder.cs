using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.ClientRoute {
    public class ClientRouteAlterationBuilder {
        private readonly Feature _feature;
        private readonly string _routeName;
        private readonly IList<Action<ClientRouteDescriptor>> _configurations = new List<Action<ClientRouteDescriptor>>();

        public ClientRouteAlterationBuilder(Feature feature, string routeName) {
            _feature = feature;
            _routeName = routeName;
        }

        public ClientRouteAlterationBuilder Configure(Action<ClientRouteDescriptor> action) {
            _configurations.Add(action);
            return this;
        }

        public ClientRouteAlterationBuilder View(Action<ClientViewDescriptor> action) {
            Configure(descriptor => {
                var viewDescriptor = new ClientViewDescriptor();
                action(viewDescriptor);
                var existedView = descriptor.Views.FirstOrDefault(
                    view => string.Equals(view.Name,viewDescriptor.Name,StringComparison.OrdinalIgnoreCase));
                if (existedView == null) {
                    descriptor.Views.Add(viewDescriptor);
                }
                else {
                    action(existedView);
                }
            });
            return this;
        }

        public ClientRouteAlteration Build() {
            return new ClientRouteAlteration(_routeName, _feature, _configurations.ToArray());
        }
    }
}