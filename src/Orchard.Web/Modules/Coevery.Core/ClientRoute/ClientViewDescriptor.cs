using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.Core.ClientRoute {
    public class ClientViewDescriptor {

        private List<string> _dependencies;

        public ClientViewDescriptor() {
            _dependencies = new List<string>();
        }

        public string Name { get; set; }

        public string TemplateUrl { get; set; }

        public string TemplateProvider { get; set; }

        public string Controller { get; set; }

        public string[] Dependencies {
            get { return _dependencies.ToArray(); }
        }

        public void AddDependencies(Func<string, string> urlTranslate, params string[] dependencies) {
            if (dependencies != null && dependencies.Any()) {
                if (urlTranslate != null) dependencies = dependencies.Select(urlTranslate).ToArray();
                _dependencies.AddRange(dependencies);
            }
        }
    }
}