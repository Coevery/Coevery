using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.ContentPicker.Fields;

namespace Coevery.ContentPicker.ViewModels {

    public class ContentPickerFieldViewModel {

        public ICollection<ContentItem> ContentItems { get; set; }
        public string SelectedIds { get; set; }
        public ContentPickerField Field { get; set; }
        public ContentPart Part { get; set; }
    }
}