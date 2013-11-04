namespace Coevery.UI.Resources {
    public interface IResourceManifestProvider : ISingletonDependency {
        void BuildManifests(ResourceManifestBuilder builder);
    }
}
