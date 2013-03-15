using Orchard.UI.Resources;

namespace Coevery.Leads
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
        
           
            manifest.DefineScript("LeadMain").SetUrl("app/main.js");
            manifest.DefineScript("LeadLogger").SetUrl("app/logger.js");
            manifest.DefineScript("LeadContext").SetUrl("app/context.js");
            manifest.DefineScript("LeadController").SetUrl("app/controller.js");
            manifest.DefineScript("LeadDetailController").SetUrl("app/detailcontroller.js");
          
        }
    }
}
