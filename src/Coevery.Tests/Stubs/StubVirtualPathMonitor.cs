using Coevery.Caching;
using Coevery.FileSystems.VirtualPath;

namespace Coevery.Tests.Stubs {
    public class StubVirtualPathMonitor : IVirtualPathMonitor {
        public class Token : IVolatileToken {
            public bool IsCurrent { get; set; }
        }
        public IVolatileToken WhenPathChanges(string virtualPath) {
            return new Token();
        }
    }
}
