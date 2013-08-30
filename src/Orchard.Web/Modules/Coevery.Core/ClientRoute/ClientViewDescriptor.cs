using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.Core.ClientRoute {
    public class ClientViewDescriptor {

        public string Name { get; set; }

        public string TemplateUrl { get; set; }

        public string TemplateProvider { get; set; }

        public string Controller { get; set; }

        public string[] Dependencies { get; set; }
    }
}