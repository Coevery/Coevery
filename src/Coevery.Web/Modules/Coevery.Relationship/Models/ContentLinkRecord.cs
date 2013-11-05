using Coevery.ContentManagement.Records;

namespace Coevery.Relationship.Models {
    public abstract class ContentLinkRecord {
        public virtual int Id { get; set; }
        public virtual ContentItemRecord PrimaryPartRecord { get; set; }
        public virtual ContentItemRecord RelatedPartRecord { get; set; }
    }
}