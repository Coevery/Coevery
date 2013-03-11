using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coevery.Dynamic {
    public class DynamicTypeDefinition {
        public string Name { get; set; }
        public IEnumerable<DynamicFieldDefinition> Fields { get; set; } 
    }
}
