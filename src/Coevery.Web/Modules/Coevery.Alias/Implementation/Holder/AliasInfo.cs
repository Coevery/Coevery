using System.Collections.Generic;

namespace Coevery.Alias.Implementation.Holder {
    public class AliasInfo {
        public string Area { get; set; }
        public string Path { get; set; }
        public IDictionary<string, string> RouteValues { get; set; }
    }
}