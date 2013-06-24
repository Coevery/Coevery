using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.ClientRoute {
    public class ClientRouteBuilder {
        private readonly IDictionary<string, ClientRouteDefinitionBuilder> _routeTable = new Dictionary<string, ClientRouteDefinitionBuilder>();

        public ClientRouteBuilder Create(string name, Feature feature, Action<ClientRouteDefinitionBuilder> build) {
            var basePath = VirtualPathUtility.AppendTrailingSlash(feature.Descriptor.Extension.Location + "/" + feature.Descriptor.Extension.Id);
            var definitionBuidler = new ClientRouteDefinitionBuilder(name, basePath);
            build(definitionBuidler);
            _routeTable.Add(name, definitionBuidler);
            return this;
        }

        public object Build() {
            IDictionary<string, object> routes = new ExpandoObject();

            _routeTable.Select(kvp => new {kvp.Key, Route = kvp.Value.Build()})
                       .ToList().ForEach(r => { routes[r.Key] = r.Route; });
            return routes;
        }
    }
}