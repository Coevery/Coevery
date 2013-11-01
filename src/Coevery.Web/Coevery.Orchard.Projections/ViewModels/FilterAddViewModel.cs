using System.Collections.Generic;
using Coevery.Orchard.Projections.Descriptors;
using Coevery.Orchard.Projections.Descriptors.Filter;

namespace Coevery.Orchard.Projections.ViewModels {
    public class FilterAddViewModel {
        public int Id { get; set; }
        public int Group { get; set; }

        public IEnumerable<TypeDescriptor<FilterDescriptor>> Filters { get; set; }
    }
}
