using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using System.IO;
using System.Web.Mvc;

namespace Coevery.Core {
    public class Shapes : IShapeTableProvider {

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Content_Edit")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    shape.Metadata.Alternates.Add("Content_Edit__Default");
                });
        }

        [Shape]
        public void ngGrid(dynamic Display, TextWriter Output, HtmlHelper Html)
        {
            Output.WriteLine("<section class=\"gridStyle\" ng-grid=\"gridOptions\"></section>");
        }
    }
}
