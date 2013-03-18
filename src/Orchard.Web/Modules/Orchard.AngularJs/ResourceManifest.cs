using Orchard.UI.Resources;

namespace Orchard.jQuery {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineStyle("TodoStyle").SetUrl("todo.css");
            manifest.DefineStyle("ngGrid").SetUrl("ng-grid.css");
            manifest.DefineStyle("pnotify").SetUrl("jquery.pnotify.css");
            manifest.DefineScript("angular").SetUrl("angular.js");
            manifest.DefineScript("angularResource").SetUrl("angular-resource.js");
            manifest.DefineScript("ngGrid").SetUrl("ng-grid-2.0.0.debug.js");
            manifest.DefineScript("pnotify").SetUrl("jquery.pnotify.min.js");
          
        }
    }
}
