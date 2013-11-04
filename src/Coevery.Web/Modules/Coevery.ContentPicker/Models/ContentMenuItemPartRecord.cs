using Coevery.ContentManagement.Records;

namespace Coevery.ContentPicker.Models {
    public class ContentMenuItemPartRecord : ContentPartRecord {
        public virtual ContentItemRecord ContentMenuItemRecord { get; set; }
    }
}