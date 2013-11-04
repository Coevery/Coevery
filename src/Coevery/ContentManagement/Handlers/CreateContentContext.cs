using Coevery.ContentManagement.Records;

namespace Coevery.ContentManagement.Handlers {
    public class CreateContentContext : ContentContextBase {
        public CreateContentContext(ContentItem contentItem) : base(contentItem) {
            ContentItemVersionRecord = contentItem.VersionRecord;
        }

        public ContentItemVersionRecord ContentItemVersionRecord { get; set; }
    }
}
