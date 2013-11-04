using System;
using NHibernate.Cfg;

namespace Coevery.Data {
    public interface ISessionConfigurationCache : ISingletonDependency {
        Configuration GetConfiguration(Func<Configuration> builder);
    }
}