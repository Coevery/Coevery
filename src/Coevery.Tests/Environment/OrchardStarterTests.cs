using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using NUnit.Framework;
using Coevery.Environment;

namespace Coevery.Tests.Environment {
    [TestFixture]
    public class CoeveryStarterTests {
        [Test]
        public void DefaultCoeveryHostInstanceReturnedByCreateHost() {
            var host = CoeveryStarter.CreateHost(b => b.RegisterInstance(new ControllerBuilder()));
            Assert.That(host, Is.TypeOf<DefaultCoeveryHost>());
        }

        [Test]
        public void ContainerResolvesServicesInSameOrderTheyAreRegistered() {
            var container = CoeveryStarter.CreateHostContainer(builder => {
                builder.RegisterType<Component1>().As<IServiceA>();
                builder.RegisterType<Component2>().As<IServiceA>();
            });
            var services = container.Resolve<IEnumerable<IServiceA>>();
            Assert.That(services.Count(), Is.EqualTo(2));
            Assert.That(services.First(), Is.TypeOf<Component1>());
            Assert.That(services.Last(), Is.TypeOf<Component2>());
        }

        [Test]
        public void MostRecentlyRegisteredServiceReturnsFromSingularResolve() {
            var container = CoeveryStarter.CreateHostContainer(builder => {
                builder.RegisterType<Component1>().As<IServiceA>();
                builder.RegisterType<Component2>().As<IServiceA>();
            });
            var service = container.Resolve<IServiceA>();
            Assert.That(service, Is.Not.Null);
            Assert.That(service, Is.TypeOf<Component2>());
        }

        public interface IServiceA {}

        public class Component1 : IServiceA {}

        public class Component2 : IServiceA {}
    }
}