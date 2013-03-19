using Orchard.UI.Resources;

namespace Coevery.Leads
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("LeadLocalize").SetUrl("LeadLocalize.js");
            manifest.DefineScript("LeadMain").SetUrl("app/app.js");
            manifest.DefineScript("LeadLogger").SetUrl("app/logger.js");
            manifest.DefineScript("LeadContext").SetUrl("app/context.js");
            manifest.DefineScript("LeadController").SetUrl("controllers/listcontroller.js");
            manifest.DefineScript("LeadDetailController").SetUrl("controllers/detailcontroller.js");
            manifest.DefineScript("LeadModel").SetUrl("models/lead.js");
        }
    }
}
