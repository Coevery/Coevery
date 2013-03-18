using Orchard.UI.Resources;

namespace Coevery.Metadata {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("ContentTypesAdmin").SetUrl("orchard-contenttypes-admin.css");
            manifest.DefineScript("localize").SetUrl("localize.js");


            manifest.DefineScript("angular").SetUrl("angular.js")
               .SetCdn("http://ajax.googleapis.com/ajax/libs/angularjs/1.0.5/angular.min.js", "http://ajax.googleapis.com/ajax/libs/angularjs/1.0.5/angular.min.js", false)
               .SetVersion("1.0.5");
            manifest.DefineScript("MetadataApp").SetUrl("app/app.js");
            manifest.DefineScript("MetadataLogger").SetUrl("app/logger.js");
            manifest.DefineScript("MetadataContext").SetUrl("app/context.js");
            manifest.DefineScript("MetadataController").SetUrl("controllers/listcontroller.js");
            manifest.DefineScript("MetadataDetailController").SetUrl("controllers/detailcontroller.js");
            manifest.DefineScript("MetadataModel").SetUrl("models/Metadata.js");
        }
    }
}
