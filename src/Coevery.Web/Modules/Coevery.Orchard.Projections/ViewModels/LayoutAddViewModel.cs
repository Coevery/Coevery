using System.Collections.Generic;
using Coevery.Orchard.Projections.Descriptors;
using Coevery.Orchard.Projections.Descriptors.Layout;

namespace Coevery.Orchard.Projections.ViewModels {
    public class LayoutAddViewModel {
        public int Id { get; set; }
        public IEnumerable<TypeDescriptor<LayoutDescriptor>> Layouts { get; set; }
    }
}
