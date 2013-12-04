using System.Collections.Generic;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Property;

namespace Coevery.Projections.ViewModels {
    public class GroupAddViewModel {
        public int Id { get; set; }
        public IEnumerable<PropertyEntry> Properties { get; set; }
    }
}
