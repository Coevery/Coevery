using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models
{
    public class ManyToManyRelationshipRecord : ContentPartRecord
    {
        public virtual int Relationship_Id { get; set; }
        public virtual string RelatedListLabel { get; set; }
        public virtual int ShowRelatedList { get; set; }//bit
        public virtual string PrimaryListLabel { get; set; }
        public virtual int ShowPrimaryList { get; set; }//bit
    }
}