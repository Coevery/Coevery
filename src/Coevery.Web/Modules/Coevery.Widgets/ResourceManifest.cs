using Coevery.UI.Resources;

namespace Coevery.Widgets {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            builder.Add().DefineStyle("WidgetsAdmin").SetUrl("coevery-widgets-admin.css").SetDependencies("~/Themes/TheAdmin/Styles/Site.css");
        }
    }
}
