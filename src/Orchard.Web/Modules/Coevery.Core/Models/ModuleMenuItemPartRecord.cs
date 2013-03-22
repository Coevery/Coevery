using Orchard.ContentManagement.Records;

namespace Coevery.Core.Models {
    public class ModuleMenuItemPartRecord : ContentPartRecord {
        public virtual ContentItemRecord ModuleMenuItemRecord { get; set; }
    }
}