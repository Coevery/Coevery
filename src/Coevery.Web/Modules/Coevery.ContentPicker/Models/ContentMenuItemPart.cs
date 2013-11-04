using Coevery.ContentManagement;
using Coevery.ContentManagement.Utilities;

namespace Coevery.ContentPicker.Models {
    public class ContentMenuItemPart : ContentPart<ContentMenuItemPartRecord> {

        public readonly LazyField<ContentItem> _content = new LazyField<ContentItem>();

        public ContentItem Content {
            get { return _content.Value;  }
            set {
                _content.Value = value; 
                Record.ContentMenuItemRecord = value == null ? null : value.Record; 
            }
        }
    }
}
