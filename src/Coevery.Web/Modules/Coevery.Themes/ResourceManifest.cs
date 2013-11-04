using Coevery.UI.Resources;

namespace Coevery.Themes {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("ThemesAdmin").SetUrl("coevery-themes-admin.css");
        }
    }
}
