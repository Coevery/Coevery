using Orchard.UI.Resources;

namespace Coevery.Leads
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("LeadContext").SetUrl("app/context.js");
            manifest.DefineScript("LeadController").SetUrl("controllers/listcontroller.js");
            manifest.DefineScript("LeadDetailController").SetUrl("controllers/detailcontroller.js");

           
        }
    }
}
