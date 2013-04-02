using Orchard.UI.Resources;

namespace Coevery.Core
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("CoeveryApp").SetUrl("app/app.js");
            manifest.DefineScript("Logger").SetUrl("app/logger.js");
            manifest.DefineScript("CommonListController").SetUrl("controllers/listcontroller.js");
            manifest.DefineScript("CommonDetailController").SetUrl("controllers/detailcontroller.js");
            manifest.DefineScript("CommonContext").SetUrl("app/context.js");
        }
    }
}
