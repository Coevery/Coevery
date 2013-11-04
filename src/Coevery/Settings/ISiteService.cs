namespace Coevery.Settings {
    public interface ISiteService : IDependency {
        ISite GetSiteSettings();
    }
}
