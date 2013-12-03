using System;
using NHibernate.Cfg;

namespace Coevery.Data {
    public interface ISessionConfigurationCache {
        Configuration GetConfiguration(Func<Configuration> builder);
    }
}