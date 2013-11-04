using Coevery.Environment.Extensions;
using Coevery.UI.Resources;

namespace Coevery.Packaging {
    [CoeveryFeature("Gallery")]
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            UI.Resources.ResourceManifest resourceManifest = builder.Add();
            resourceManifest.DefineStyle("PackagingAdmin").SetUrl("coevery-packaging-admin.css");
        }
    }
}
