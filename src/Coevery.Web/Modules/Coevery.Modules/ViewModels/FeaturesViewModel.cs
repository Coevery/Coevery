using System.Collections.Generic;
using Coevery.Modules.Models;

namespace Coevery.Modules.ViewModels {
    public class FeaturesViewModel {
        public IEnumerable<ModuleFeature> Features { get; set; }
        public FeaturesBulkAction BulkAction { get; set; }
    }

    
}