using Coevery.DisplayManagement.Descriptors;
using Coevery.Projections.Descriptors.Filter;

namespace Coevery.QuickFilters {
    public class QuickFilterShape : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("QuickFilter_ReferenceFilter")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    FilterDescriptor filter = shape.Filter;
                    string filterType = filter.Type;
                    var args = filterType.Split('.');
                    string partName = args[0];
                    string fieldName = args[1];

                    shape.PartName = partName;
                    shape.FieldName = fieldName;
                });

            builder.Describe("QuickFilter_OptionSetFilter")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    FilterDescriptor filter = shape.Filter;
                    string filterType = filter.Type;
                    var args = filterType.Split('.');
                    string partName = args[0];
                    string fieldName = args[1];

                    shape.PartName = partName;
                    shape.FieldName = fieldName;
                });
        }
    }
}