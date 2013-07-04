using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Coevery.Core.ClientRoute {
    public class ClientRouteDescriptor {
        private string[] _dependencies;

        public bool UseDefaultUrl { get; set; }
        public bool? Abstract { get; set; }

        public string RouteName { get; set; }

        public string Url { get; set; }

        public string BaseUrl { get; set; }

        public string TemplateUrl { get; set; }

        public string TemplateProvider { get; set; }

        public string Controller { get; set; }

        public string[] Dependencies {
            get { return _dependencies; }
            set {
                _dependencies = ToClientUrl(value);
            }
        }


        private string[] ToClientUrl(IEnumerable<string> scripts) {
            if (scripts == null) return null;
            var results = scripts.Select(scriptPath => VirtualPathUtility.Combine(VirtualPathUtility.Combine(BaseUrl, "Scripts/"), scriptPath + ".js"))
                                 .Select(VirtualPathUtility.ToAbsolute).ToArray();
            return results;
        }
    }
}