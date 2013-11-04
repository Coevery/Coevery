using System;
using NHibernate;
using Coevery.Data;

namespace Coevery.Tests.Data {
    public class StubLocator : ISessionLocator {
        private readonly ISession _session;

        public StubLocator(ISession session) {
            _session = session;
        }

        #region ISessionLocator Members

        public ISession For(Type entityType) {
            return _session;
        }

        #endregion
    }
}