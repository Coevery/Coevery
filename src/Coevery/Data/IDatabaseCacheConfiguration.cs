using NHibernate.Cfg.Loquacious;

namespace Coevery.Data {
    public interface IDatabaseCacheConfiguration : IDependency {
        void Configure(ICacheConfigurationProperties cache);
    }
}