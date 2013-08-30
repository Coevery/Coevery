using Orchard.UI.Resources;

namespace Coevery.OptionSet
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("OptionItem").SetUrl("optionitemeditcontroller.js");
        }
    }
}
