using System.Collections.Generic;

namespace Coevery.Metadata {
    public class DynamicTypeDefinition {
        public string Name { get; set; }
        public IEnumerable<DynamicFieldDefinition> Fields { get; set; } 
    }
}
