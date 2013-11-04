using Coevery.ContentManagement.Records;

namespace Coevery.ContentManagement.Handlers {
    public class UpdateContentContext : ContentContextBase {
        public UpdateContentContext(ContentItem contentItem) : base(contentItem) {
            UpdatingItemVersionRecord = contentItem.VersionRecord;
        }

        public ContentItemVersionRecord UpdatingItemVersionRecord { get; set; }
    }
}