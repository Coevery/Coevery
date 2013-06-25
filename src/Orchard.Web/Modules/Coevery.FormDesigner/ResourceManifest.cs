using Orchard.UI.Resources;

namespace Coevery.FormDesigner {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("FormDesignerMain").SetUrl("main.js");
            manifest.DefineScript("FormDesignerContextMenu").SetUrl("contextmenu.js");
            manifest.DefineScript("FormDesignerJsutils").SetUrl("jsutils.js");
            manifest.DefineScript("FormDesignerLinq").SetUrl("linq.js");

            manifest.DefineStyle("Affix").SetUrl("docs.css");
        }
    }
}
