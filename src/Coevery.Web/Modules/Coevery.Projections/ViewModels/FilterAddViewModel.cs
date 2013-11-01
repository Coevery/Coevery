using System.Collections.Generic;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Filter;

namespace Coevery.Projections.ViewModels {
    public class FilterAddViewModel {
        public int Id { get; set; }
        public int Group { get; set; }

        public IEnumerable<TypeDescriptor<FilterDescriptor>> Filters { get; set; }
    }
}
