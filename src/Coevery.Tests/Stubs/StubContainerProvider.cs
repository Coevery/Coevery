using Autofac;
using Autofac.Integration.Web;

namespace Coevery.Tests.Stubs {
    public class StubContainerProvider : IContainerProvider {
        public StubContainerProvider(IContainer applicationContainer, ILifetimeScope requestContainer) {
            ApplicationContainer = applicationContainer;
            RequestLifetime = requestContainer;
        }

        public void EndRequestLifetime() {
            RequestLifetime.Dispose();
        }

        public ILifetimeScope ApplicationContainer { get; set; }

        public ILifetimeScope RequestLifetime { get; set; }
    }
}