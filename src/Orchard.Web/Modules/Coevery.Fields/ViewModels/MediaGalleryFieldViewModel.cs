using System.Collections.Generic;
using Coevery.Fields.Fields;

namespace Coevery.Fields.ViewModels {

    public class MediaGalleryFieldViewModel {

        public ICollection<MediaGalleryItem> Items { get; set; }
        public string SelectedItems { get; set; }
        public MediaGalleryField Field { get; set; }
    }
}