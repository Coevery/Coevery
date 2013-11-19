using Coevery.DisplayManagement.Descriptors;

namespace Coevery.Common {
    public class ListViewShape : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Content_Edit")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    shape.Metadata.Alternates.Add("Content_Edit__Default");
                });

            builder.Describe("Content")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    shape.Metadata.Alternates.Add("Content__Default");
                });
        }
    }
}