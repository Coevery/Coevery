using System.Collections.Generic;
using Coevery.Orchard.Projections.Descriptors;
using Coevery.Orchard.Projections.Descriptors.SortCriterion;

namespace Coevery.Orchard.Projections.ViewModels {
    public class SortCriterionAddViewModel {
        public int Id { get; set; }
        public IEnumerable<TypeDescriptor<SortCriterionDescriptor>> SortCriteria { get; set; }
    }
}
