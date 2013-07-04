using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.ClientRoute {
    public class ClientRouteTableBuilder {

        private readonly IList<ClientRouteAlterationBuilder> _alterationBuilders = new List<ClientRouteAlterationBuilder>();
        private readonly Feature _feature;

        public ClientRouteTableBuilder(Feature feature) {
            _feature = feature;
        }

        public ClientRouteAlterationBuilder Describe(string shapeType) {
            var alterationBuilder = new ClientRouteAlterationBuilder(_feature, shapeType);
            _alterationBuilders.Add(alterationBuilder);
            return alterationBuilder;
        }

        public IEnumerable<ClientRouteAlteration> BuildAlterations() {
            return _alterationBuilders.Select(b => b.Build());
        }

        //private readonly IDictionary<string, ClientRouteDefinitionBuilder> _routeTable = new Dictionary<string, ClientRouteDefinitionBuilder>();

        //public ClientRouteBuilder Create(string name, Feature feature, Action<ClientRouteDefinitionBuilder> build) {
        //    var basePath = VirtualPathUtility.AppendTrailingSlash(feature.Descriptor.Extension.Location + "/" + feature.Descriptor.Extension.Id);
        //    var definitionBuidler = new ClientRouteDefinitionBuilder(name, basePath);
        //    build(definitionBuidler);
        //    _routeTable.Add(name, definitionBuidler);
        //    return this;
        //}

        //public object Build() {
        //    IDictionary<string, object> routes = new ExpandoObject();

        //    _routeTable.Select(kvp => new {kvp.Key, Route = kvp.Value.Build()})
        //               .ToList().ForEach(r => { routes[r.Key] = r.Route; });
        //    return routes;
        //}
    }
}