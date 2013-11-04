using Coevery.UI.Resources;

namespace Coevery.Autoroute {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("AutorouteSettings").SetUrl("coevery-autoroute-settings.css");
        }
    }
}
