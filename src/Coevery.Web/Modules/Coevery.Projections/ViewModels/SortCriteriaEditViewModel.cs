using Coevery.Projections.Descriptors.SortCriterion;

namespace Coevery.Projections.ViewModels {

    public class SortCriterionEditViewModel {
        public int Id { get; set; }
        public string Description { get; set; }
        public SortCriterionDescriptor SortCriterion { get; set; }
        public dynamic Form { get; set; }
    }
}
