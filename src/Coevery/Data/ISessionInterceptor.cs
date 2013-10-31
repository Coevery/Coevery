using NHibernate;

namespace Coevery.Data {
    /// <summary>
    /// Describes an NHibernate session interceptor, instantiated per-session.
    /// </summary>
    public interface ISessionInterceptor : IInterceptor, IDependency {
    }
}