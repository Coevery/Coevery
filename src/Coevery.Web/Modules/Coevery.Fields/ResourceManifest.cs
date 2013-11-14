using Coevery.UI.Resources;

namespace Coevery.Fields {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            //Bootstrap datetimepicker
            manifest.DefineScript("Bootstrap_Datetimepicker").SetUrl("bootstrap-datetimepicker.min.js").SetDependencies("jQuery");
            manifest.DefineStyle("Bootstrap_Datetimepicker").SetUrl("bootstrap-datetimepicker.min.css");
        }
    }
}
