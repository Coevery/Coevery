using System.Web.Routing;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Mvc;

namespace Coevery.Core {
    public class CoreShapes : IShapeTableProvider {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CoreShapes(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Layout")
                   .OnDisplaying(displaying =>
                   {
                       IShape layout = displaying.Shape;
                       var httpContext = _httpContextAccessor.Current();
                       var routeValues = httpContext.Request.RequestContext.RouteData.Values;
                       var controller = (string)routeValues["controller"];
                       if (controller == "SystemAdmin")
                           layout.Metadata.Alternates.Add("Layout__" + controller);
                   });

            builder.Describe("Menu")
                   .OnDisplaying(displaying =>
                   {
                       IShape layout = displaying.Shape;
                       var httpContext = _httpContextAccessor.Current();
                       var routeValues = httpContext.Request.RequestContext.RouteData.Values;
                       var controller = (string)routeValues["controller"];
                       if (controller == "SystemAdmin")
                           layout.Metadata.Alternates.Add("Menu__" + controller);
                   });
        }
    }
}