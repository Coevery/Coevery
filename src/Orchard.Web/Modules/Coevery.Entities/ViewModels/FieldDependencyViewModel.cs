using System.Collections.Generic;

namespace Coevery.Entities.ViewModels {
    public class FieldDependencyViewModel {
        public FieldDependencyViewModel() {
            ControlFields = new List<EditPartFieldViewModel>();
            DependentFields = new List<EditPartFieldViewModel>();
        }

        public IEnumerable<EditPartFieldViewModel> ControlFields { get; set; }
        public IEnumerable<EditPartFieldViewModel> DependentFields { get; set; }
    }
}