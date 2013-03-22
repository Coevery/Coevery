using Orchard.UI.Resources;

namespace Coevery.Leads
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("CoeveryApp").SetUrl("app/app.js");
            manifest.DefineScript("Logger").SetUrl("app/logger.js");
            manifest.DefineScript("states").SetUrl("angular-ui-states.js");

            manifest.DefineScript("LeadContext").SetUrl("app/context.js");
            manifest.DefineScript("LeadController").SetUrl("controllers/listcontroller.js");
            manifest.DefineScript("LeadDetailController").SetUrl("controllers/detailcontroller.js");

            manifest.DefineScript("FormDesignerMain").SetUrl("formdesigner/main.js");
            manifest.DefineScript("FormDesignerContextMenu").SetUrl("formdesigner/ContextMenu.js");
            manifest.DefineScript("FormDesignerJsutils").SetUrl("formdesigner/jsutils.js");
            manifest.DefineScript("FormDesignerLinq").SetUrl("formdesigner/Linq.js");
        }
    }
}
