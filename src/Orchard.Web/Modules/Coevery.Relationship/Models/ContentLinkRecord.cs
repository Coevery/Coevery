using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models {
    public interface IContentLinkRecord {
        int Id { get; set; }
        ContentPartRecord PrimaryPartRecord { get; set; }
        ContentPartRecord RelatedPartRecord { get; set; }
    }
    //public class ContentLinkRecord {
    //    public virtual int Id { get; set; }
    //    public virtual ContentPartRecord PrimaryPartRecord { get; set; }
    //    public virtual ContentPartRecord RelatedPartRecord { get; set; }
    //}
}