using System.Web.Routing;
using Coevery.Web.Tests.Stubs;
using NUnit.Framework;

namespace Coevery.Web.Tests.Routes {
    [TestFixture]
    public class RouteTests {
        [Test]
        public void RouteForEmbeddedResource() {
            // Arrange
            var context = new StubContext("~/foo.axd/bar/baz/biff");
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            // Act
            var routeData = routes.GetRouteData(context);

            // Assert
            Assert.That(routeData, Is.Not.Null);
            Assert.That(routeData.RouteHandler, Is.TypeOf<StopRoutingHandler>());
        }

        [Test]
        public void RouteWithTooManySegments() {
            // Arrange
            var context = new StubContext("~/a/b/c/d");
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            // Act
            var routeData = routes.GetRouteData(context);

            // Assert
            Assert.That(routeData, Is.Null);
        }
    }
}