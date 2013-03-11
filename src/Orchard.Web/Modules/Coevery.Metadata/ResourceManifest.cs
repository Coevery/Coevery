using Orchard.UI.Resources;

namespace Coevery.Metadata {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            builder.Add().DefineStyle("ContentTypesAdmin").SetUrl("orchard-contenttypes-admin.css");
        }
    }
}
