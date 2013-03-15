using Orchard.UI.Resources;

namespace Coevery.Opportunities
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineScript("OpportunityMain").SetUrl("app/main.js");
            manifest.DefineScript("OpportunityLogger").SetUrl("app/logger.js");
            manifest.DefineScript("OpportunityContext").SetUrl("app/context.js");
            manifest.DefineScript("OpportunityController").SetUrl("app/controller.js");
            manifest.DefineScript("OpportunityDetailController").SetUrl("app/detailcontroller.js");
        }
    }
}
