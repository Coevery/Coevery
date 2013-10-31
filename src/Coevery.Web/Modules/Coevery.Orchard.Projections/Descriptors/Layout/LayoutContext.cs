using System.Collections.Generic;
using Coevery.Orchard.Projections.Models;

namespace Coevery.Orchard.Projections.Descriptors.Layout {
    public class LayoutContext {
        public dynamic State { get; set; }
        public LayoutRecord LayoutRecord { get; set; }
    }
}