using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Moq;
using NUnit.Framework;
using Coevery.Caching;
using Coevery.DisplayManagement;
using Coevery.DisplayManagement.Descriptors;
using Coevery.DisplayManagement.Descriptors.ShapeAttributeStrategy;
using Coevery.DisplayManagement.Implementation;
using Coevery.DisplayManagement.Shapes;
using Coevery.Environment;
using Coevery.Environment.Extensions.Models;
using Coevery.Tests.Stubs;
using Coevery.Tests.Utility;

namespace Coevery.Tests.DisplayManagement {
    [TestFixture]
    public class SubsystemTests {
        private IContainer _container;

        [SetUp]
        public void Init() {
            var testFeature = new Feature
            {
                Descriptor = new FeatureDescriptor
                {
                    Id = "Testing",
                    Extension = new ExtensionDescriptor
                    {
                        Id = "Testing",
                        ExtensionType = DefaultExtensionTypes.Module,
                    }
                }
            };

            var workContext = new DefaultDisplayManagerTests.TestWorkContext
            {
                CurrentTheme = new ExtensionDescriptor { Id = "Hello" }
            };

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ShapeAttributeBindingModule());
            builder.RegisterType<ShapeAttributeBindingStrategy>().As<IShapeTableProvider>();
            builder.RegisterType<ShapeTableLocator>().As<IShapeTableLocator>();
            builder.RegisterType<DefaultDisplayManager>().As<IDisplayManager>();
            builder.RegisterType<DefaultShapeFactory>().As<IShapeFactory>();
            builder.RegisterType<DisplayHelperFactory>().As<IDisplayHelperFactory>();
            builder.RegisterType<DefaultShapeTableManager>().As<IShapeTableManager>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<StubParallelCacheContext>().As<IParallelCacheContext>();
            builder.RegisterInstance(new DefaultDisplayManagerTests.TestWorkContextAccessor(workContext)).As<IWorkContextAccessor>();
            builder.RegisterInstance(new SimpleShapes()).WithMetadata("Feature", testFeature);
            builder.RegisterInstance(new RouteCollection());
            builder.RegisterAutoMocking(MockBehavior.Loose);

            _container = builder.Build();
            _container.Resolve<Mock<ICoeveryHostContainer>>()
                .Setup(x => x.Resolve<IComponentContext>())
                .Returns(_container);
        }

        public class SimpleShapes {
            [Shape]
            public IHtmlString Something() {
                return new HtmlString("<br/>");
            }

            [Shape]
            public IHtmlString Pager() {
                return new HtmlString("<div>hello</div>");
            }
        }

        [Test]
        public void RenderingSomething() {
            dynamic displayHelperFactory = _container.Resolve<IDisplayHelperFactory>().CreateHelper(new ViewContext(), null);
            dynamic shapeHelperFactory = _container.Resolve<IShapeFactory>();

            var result1 = displayHelperFactory.Something();
            var result2 = ((DisplayHelper)displayHelperFactory).ShapeExecute(shapeHelperFactory.Pager());
            var result3 = ((DisplayHelper)displayHelperFactory).ShapeExecute((Shape)shapeHelperFactory.Pager());

            displayHelperFactory(shapeHelperFactory.Pager());

            Assert.That(result1.ToString(), Is.EqualTo("<br/>"));
            Assert.That(result2.ToString(), Is.EqualTo("<div>hello</div>"));
            Assert.That(result3.ToString(), Is.EqualTo("<div>hello</div>"));
        }
    }
}