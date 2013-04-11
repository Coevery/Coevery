using Orchard.UI.Resources;

namespace Coevery.CoeveryNavigation
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineScript("NavigationListCtrl").SetUrl("controllers/navigationListcontroller.js");
            manifest.DefineScript("NavigationDetailCtrl").SetUrl("controllers/navigationdetailcontroller.js");
        }
    }
}
