using Orchard.DisplayManagement.Descriptors;

namespace Coevery.Core {
    public class Shapes : IShapeTableProvider {

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Content_Edit")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    shape.Metadata.Alternates.Add("Content_Edit__Default");
                });
        }
    }
}
