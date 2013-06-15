using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.MetaData;

namespace Coevery.Metadata.ViewModels {
    public class FieldDependencyViewModel {
        public FieldDependencyViewModel() {
            ControlFields = new List<EditPartFieldViewModel>();
            DependentFields = new List<EditPartFieldViewModel>();
        }

        public IEnumerable<EditPartFieldViewModel> ControlFields { get; set; }
        public IEnumerable<EditPartFieldViewModel> DependentFields { get; set; }
    }
}