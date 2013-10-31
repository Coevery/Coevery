using Coevery.ContentManagement.Records;
using Coevery.Core.Settings.Metadata.Records;

namespace Coevery.Common.Models {
    public class ModuleMenuItemPartRecord : ContentPartRecord {
        public virtual ContentTypeDefinitionRecord ContentTypeDefinitionRecord { get; set; }
        public virtual string IconClass { get; set; }
    }
}