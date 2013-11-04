using Coevery.UI.Resources;

namespace Coevery.PublishLater {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            builder.Add().DefineStyle("PublishLater_DatePicker").SetUrl("coevery-publishlater-datetime.css");
        }
    }
}
