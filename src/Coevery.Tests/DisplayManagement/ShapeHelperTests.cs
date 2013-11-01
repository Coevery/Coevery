using Autofac;
using NUnit.Framework;
using Coevery.Caching;
using Coevery.DisplayManagement;
using Coevery.DisplayManagement.Descriptors;
using Coevery.DisplayManagement.Implementation;
using Coevery.Environment.Extensions;
using Coevery.Tests.Stubs;

namespace Coevery.Tests.DisplayManagement {
    [TestFixture]
    public class ShapeHelperTests {
        private IContainer _container;

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            builder.RegisterType<DefaultShapeFactory>().As<IShapeFactory>();
            builder.RegisterType<DefaultShapeTableManager>().As<IShapeTableManager>();
            builder.RegisterType<ShapeTableLocator>().As<IShapeTableLocator>();
            builder.RegisterType<StubExtensionManager>().As<IExtensionManager>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<StubParallelCacheContext>().As<IParallelCacheContext>();
            _container = builder.Build();

        }

        [Test]
        public void CreatingNewShapeTypeByName() {
            dynamic shape = _container.Resolve<IShapeFactory>();

            var alpha = shape.Alpha();

            Assert.That(alpha.Metadata.Type, Is.EqualTo("Alpha"));
        }

        [Test]
        public void CreatingShapeWithAdditionalNamedParameters() {
            dynamic shape = _container.Resolve<IShapeFactory>();

            var alpha = shape.Alpha(one: 1, two: "dos");

            Assert.That(alpha.Metadata.Type, Is.EqualTo("Alpha"));
            Assert.That(alpha.one, Is.EqualTo(1));
            Assert.That(alpha.two, Is.EqualTo("dos"));
        }

        [Test]
        public void WithPropertyBearingObjectInsteadOfNamedParameters() {
            dynamic shape = _container.Resolve<IShapeFactory>();

            var alpha = shape.Alpha(new { one = 1, two = "dos" });

            Assert.That(alpha.Metadata.Type, Is.EqualTo("Alpha"));
            Assert.That(alpha.one, Is.EqualTo(1));
            Assert.That(alpha.two, Is.EqualTo("dos"));
        }
    }
}
