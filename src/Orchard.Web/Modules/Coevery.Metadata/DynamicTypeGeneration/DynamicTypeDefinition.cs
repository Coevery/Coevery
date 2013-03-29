using System.Collections.Generic;

namespace Coevery.Metadata.DynamicTypeGeneration {
    public class DynamicTypeDefinition {
        public string Name { get; set; }
        public IEnumerable<DynamicFieldDefinition> Fields { get; set; } 
    }
}
