using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.ClientRoute {
    public class ClientRouteAlteration {
        private readonly IList<Action<ClientRouteDescriptor>> _configurations;

        public ClientRouteAlteration(string routeName, Feature feature, IList<Action<ClientRouteDescriptor>> configurations) {
            _configurations = configurations;
            RouteName = routeName;
            Feature = feature;
        }

        public string RouteName { get; private set; }
        public Feature Feature { get; private set; }
        public void Alter(ClientRouteDescriptor descriptor) {
            foreach (var configuration in _configurations) {
                configuration(descriptor);
            }
        }
    }
}