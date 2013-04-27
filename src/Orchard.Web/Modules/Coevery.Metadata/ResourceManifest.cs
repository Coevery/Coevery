using Orchard.UI.Resources;

namespace Coevery.Metadata {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("MetadataContext").SetUrl("app/context.js");
            manifest.DefineScript("MetadataController").SetUrl("controllers/listcontroller.js");
            manifest.DefineScript("MetadataDetailController").SetUrl("controllers/detailcontroller.js");
            manifest.DefineScript("MetadataEditController").SetUrl("controllers/editcontroller.js");
            manifest.DefineScript("FieldCtrl").SetUrl("controllers/fieldlistcontroller.js");
            manifest.DefineScript("FieldDetailCtrl").SetUrl("controllers/fielddetailcontroller.js");

            manifest.DefineScript("FormDesignerMain").SetUrl("formdesigner/main.js");
            manifest.DefineScript("FormDesignerContextMenu").SetUrl("formdesigner/contextmenu.js");
            manifest.DefineScript("FormDesignerJsutils").SetUrl("formdesigner/jsutils.js");
            manifest.DefineScript("FormDesignerLinq").SetUrl("formdesigner/linq.js");

            manifest.DefineScript("UserViewListController").SetUrl("controllers/userviewlistcontroller.js");
            manifest.DefineScript("UserViewDetailController").SetUrl("controllers/userviewdetailcontroller.js");
            manifest.DefineStyle("ColumnSelector").SetUrl("formdesigner/css/columnselector.css");
        }
    }
}
