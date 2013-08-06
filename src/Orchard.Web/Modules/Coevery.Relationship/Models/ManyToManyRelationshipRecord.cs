using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models
{
    public class ManyToManyRelationshipRecord : ContentPartRecord
    {
        public virtual int Id { get; set; }
        public virtual RelationshipRecord Relationship { get; set; }
        public virtual string RelatedListLabel { get; set; }
        public virtual bool ShowRelatedList { get; set; }//bit
        public virtual string PrimaryListLabel { get; set; }
        public virtual bool ShowPrimaryList { get; set; }//bit
    }
}