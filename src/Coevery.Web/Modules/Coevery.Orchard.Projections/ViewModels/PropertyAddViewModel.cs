using System.Collections.Generic;
using Coevery.Orchard.Projections.Descriptors;
using Coevery.Orchard.Projections.Descriptors.Property;

namespace Coevery.Orchard.Projections.ViewModels {
    public class PropertyAddViewModel {
        public int Id { get; set; }
        public IEnumerable<TypeDescriptor<PropertyDescriptor>> Properties { get; set; }
    }
}
