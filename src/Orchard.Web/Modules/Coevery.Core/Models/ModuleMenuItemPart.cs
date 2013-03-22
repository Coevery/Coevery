using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Core.Navigation.Models;

namespace Coevery.Core.Models {
    public class ModuleMenuItemPart : ContentPart<ModuleMenuItemPartRecord>
    {
        public readonly LazyField<ContentItem> _content = new LazyField<ContentItem>();

        public ContentItem Content {
            get { return _content.Value;  }
            set {
                _content.Value = value; 
                Record.ModuleMenuItemRecord = (value == null ? null : value.Record); 
            }
        }
    }
}
