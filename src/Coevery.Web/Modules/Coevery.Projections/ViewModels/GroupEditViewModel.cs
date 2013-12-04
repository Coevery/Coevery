using System.Collections.Generic;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Property;

namespace Coevery.Projections.ViewModels {
    public class GroupEditViewModel {
        public int Id { get; set; }
        public PropertyDescriptor Property { get; set; }
        public string Sort { get; set; }
    }
}
