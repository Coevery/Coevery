using System.Collections.Generic;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Layout;

namespace Coevery.Projections.ViewModels {
    public class LayoutAddViewModel {
        public int Id { get; set; }
        public IEnumerable<TypeDescriptor<LayoutDescriptor>> Layouts { get; set; }
    }
}
