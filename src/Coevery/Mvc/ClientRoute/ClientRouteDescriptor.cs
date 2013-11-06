using System.Collections.Generic;
using System.Linq;

namespace Coevery.Mvc.ClientRoute
{
    public class ClientRouteDescriptor {

        private readonly List<ClientViewDescriptor> _views = new List<ClientViewDescriptor>();

        public bool UseDefaultUrl { get; set; }

        public bool? Abstract { get; set; }

        public string RouteName { get; set; }

        public string Url { get; set; }

        //public string TemplateUrl { get; set; }

        //public string TemplateProvider { get; set; }

        //public string Controller { get; set; }

        public List<ClientViewDescriptor> Views {
            get { return _views; }
        }

        public string[] Dependencies {
            get {
                var scripts = _views.SelectMany(v => v.Dependencies).ToArray();
                return scripts;
            }
        }
    }
}