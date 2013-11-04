using NUnit.Framework;
using Coevery.Caching;
using Coevery.Environment;
using Autofac;
using Coevery.FileSystems.AppData;
using Coevery.FileSystems.WebSite;
using Coevery.Services;

namespace Coevery.Tests.Caching {
    [TestFixture]
    public class CacheScopeTests {
        private IContainer _hostContainer;

        [SetUp]
        public void Init() {
            _hostContainer = CoeveryStarter.CreateHostContainer(builder => {
                builder.RegisterType<Alpha>().InstancePerDependency();
            });

        }

        public class Alpha {
            public ICacheManager CacheManager { get; set; }

            public Alpha(ICacheManager cacheManager) {
                CacheManager = cacheManager;
            }
        }

        [Test]
        public void ComponentsAtHostLevelHaveAccessToCache() {
            var alpha = _hostContainer.Resolve<Alpha>();
            Assert.That(alpha.CacheManager, Is.Not.Null);
        }

        [Test]
        public void HostLevelHasAccessToGlobalVolatileProviders() {
            Assert.That(_hostContainer.Resolve<IWebSiteFolder>(), Is.Not.Null);
            Assert.That(_hostContainer.Resolve<IAppDataFolder>(), Is.Not.Null);
            Assert.That(_hostContainer.Resolve<IClock>(), Is.Not.Null);
        }

    }
}
