using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Coevery.Core.ClientRoute {
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