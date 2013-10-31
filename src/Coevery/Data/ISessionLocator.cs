using System;
using NHibernate;

namespace Coevery.Data {
    public interface ISessionLocator : IDependency {        
        ISession For(Type entityType);
    }
}