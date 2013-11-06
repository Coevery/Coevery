using System.IO;
using Coevery.DisplayManagement;
using Coevery.DisplayManagement.Descriptors;
using Coevery.Mvc;
using Coevery.Mvc.ClientRoute;

namespace Coevery.Common {
    public class CoreShapes : IShapeTableProvider {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClientRouteTableManager _clientRouteTableManager;

        public CoreShapes(IHttpContextAccessor httpContextAccessor,
            IClientRouteTableManager clientRouteTableManager) {
            _httpContextAccessor = httpContextAccessor;
            _clientRouteTableManager = clientRouteTableManager;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Layout")
                .OnDisplaying(displaying => {
                    IShape layout = displaying.Shape;
                    var httpContext = _httpContextAccessor.Current();
                    var routeValues = httpContext.Request.RequestContext.RouteData.Values;
                    var controller = (string) routeValues["controller"];
                    if (controller == "SystemAdmin") {
                        layout.Metadata.Alternates.Add("Layout__" + controller);
                    }
                });

            builder.Describe("Menu")
                .OnDisplaying(displaying => {
                    IShape layout = displaying.Shape;
                    var httpContext = _httpContextAccessor.Current();
                    var routeValues = httpContext.Request.RequestContext.RouteData.Values;
                    var controller = (string) routeValues["controller"];
                    if (controller == "SystemAdmin") {
                        layout.Metadata.Alternates.Add("Menu__" + controller);
                    }
                });
        }

        [Shape]
        public void ClientRoute(dynamic Display, dynamic Shape, TextWriter Output) {
            var isFrontEnd = Shape.IsFrontEnd;
            var routes = _clientRouteTableManager.GetRouteTable(isFrontEnd);
            var result = Display.ClientBootstrapScript(IsFrontEnd: isFrontEnd, Routes: routes);
            Output.Write(result);
        }
    }
}