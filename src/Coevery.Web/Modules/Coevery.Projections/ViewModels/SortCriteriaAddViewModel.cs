using System.Collections.Generic;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.SortCriterion;

namespace Coevery.Projections.ViewModels {
    public class SortCriterionAddViewModel {
        public int Id { get; set; }
        public IEnumerable<TypeDescriptor<SortCriterionDescriptor>> SortCriteria { get; set; }
    }
}
