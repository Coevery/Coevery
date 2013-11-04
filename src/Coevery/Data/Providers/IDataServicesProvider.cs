using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;

namespace Coevery.Data.Providers {
    public interface IDataServicesProvider : ITransientDependency {
        Configuration BuildConfiguration(SessionFactoryParameters sessionFactoryParameters);
        IPersistenceConfigurer GetPersistenceConfigurer(bool createDatabase);
    }
}
