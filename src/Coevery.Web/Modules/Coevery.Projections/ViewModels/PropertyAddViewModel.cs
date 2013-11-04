using System.Collections.Generic;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Property;

namespace Coevery.Projections.ViewModels {
    public class PropertyAddViewModel {
        public int Id { get; set; }
        public IEnumerable<TypeDescriptor<PropertyDescriptor>> Properties { get; set; }
    }
}
