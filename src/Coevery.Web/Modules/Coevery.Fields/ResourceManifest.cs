using Coevery.UI.Resources;

namespace Coevery.Fields {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            //Bootstrap datetimepicker
            manifest.DefineScript("Bootstrap_Datetimepicker").SetUrl("bootstrap-datetimepicker.min.js", "bootstrap-datetimepicker.js").SetDependencies("jQuery");
            manifest.DefineStyle("Bootstrap_Datetimepicker").SetUrl("bootstrap-datetimepicker.min.css","bootstrap-datetimepicker.min.css");
        }
    }
}
