using Coevery.UI.Resources;

namespace Coevery.Modules {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            builder.Add().DefineStyle("ModulesAdmin").SetUrl("coevery-modules-admin.css");
        }
    }
}
