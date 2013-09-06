using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models {
    public abstract class ContentLinkRecord<TPrimaryPartRecord, TRelatedPartRecord>
        where TPrimaryPartRecord : ContentPartRecord
        where TRelatedPartRecord : ContentPartRecord {
        public virtual int Id { get; set; }
        public virtual TPrimaryPartRecord PrimaryPartRecord { get; set; }
        public virtual TRelatedPartRecord RelatedPartRecord { get; set; }
    }
}