using Orchard.ContentManagement.Records;
using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Core.Models {
    public class ModuleMenuItemPartRecord : ContentPartRecord {
        public virtual ContentTypeDefinitionRecord ContentTypeDefinitionRecord { get; set; }
        public virtual string IconClass { get; set; }
    }
}