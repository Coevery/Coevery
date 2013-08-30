using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Models;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.ClientRoute {
    public interface IClientRouteProvider : IDependency {
        Feature Feature { get; }
        void Discover(ClientRouteTableBuilder builder);
    }

    public abstract class ClientRouteProviderBase : IClientRouteProvider {

        public Feature Feature { get; set; }

        public abstract void Discover(ClientRouteTableBuilder builder);

        protected string[] ToClientUrl(IEnumerable<string> scripts) {
            var basePath = VirtualPathUtility.AppendTrailingSlash(Feature.Descriptor.Extension.Location + "/" + Feature.Descriptor.Extension.Id);
            if (scripts == null) return null;
            var results = scripts.Select(scriptPath =>
                VirtualPathUtility.Combine(VirtualPathUtility.Combine(basePath, "Scripts/"), scriptPath + ".js"))
                .Select(VirtualPathUtility.ToAbsolute).ToArray();
            return results;
        }
    }
}