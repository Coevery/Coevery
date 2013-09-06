using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.ClientRoute {
    public class ClientRouteTableBuilder {

        private readonly Dictionary<string, ClientRouteAlterationBuilder> _alterationBuilders = new Dictionary<string, ClientRouteAlterationBuilder>();
        private readonly Feature _feature;

        public ClientRouteTableBuilder(Feature feature) {
            _feature = feature;
        }

        public ClientRouteAlterationBuilder Describe(string routeName) {
            ClientRouteAlterationBuilder alterationBuilder;
            if (!_alterationBuilders.TryGetValue(routeName, out alterationBuilder)) {
                alterationBuilder = new ClientRouteAlterationBuilder(_feature, routeName);
                _alterationBuilders.Add(routeName, alterationBuilder);
            }
            return alterationBuilder;
        }

        public IEnumerable<ClientRouteAlteration> BuildAlterations() {
            return _alterationBuilders.Values.Select(b => b.Build());
        }
    }
}