using Orchard.ContentManagement.Records;

namespace Coevery.Core.Models {
    public class ModuleMenuItemPartRecord : ContentPartRecord {
        public virtual ContentTypeRecord ContentTypeRecord { get; set; }
    }
}