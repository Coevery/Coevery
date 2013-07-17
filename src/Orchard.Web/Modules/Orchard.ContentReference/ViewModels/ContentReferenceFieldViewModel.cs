using System.Web.Mvc;
using Contrib.ContentReference.Fields;

namespace Contrib.ContentReference.ViewModels {
    public class ContentReferenceFieldViewModel {
        public ContentReferenceField Field { get; set; }
        public SelectList ItemList { get; set; }
        public int? ContentId { get; set; }
    }
}
