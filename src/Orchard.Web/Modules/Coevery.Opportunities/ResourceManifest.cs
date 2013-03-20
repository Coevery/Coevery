using Orchard.UI.Resources;

namespace Coevery.Opportunities
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineScript("OpportunityContext").SetUrl("app/context.js");
            manifest.DefineScript("OpportunityController").SetUrl("controllers/listcontroller.js");
            manifest.DefineScript("OpportunityDetailController").SetUrl("controllers/detailcontroller.js");
            manifest.DefineScript("OpportuntityModel").SetUrl("models/opportunity.js");
        }
    }
}
