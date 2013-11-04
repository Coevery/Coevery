using Coevery.UI.Resources;

namespace Coevery.Core.Settings {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            builder.Add().DefineStyle("SettingsAdmin").SetUrl("admin.css");
        }
    }
}
