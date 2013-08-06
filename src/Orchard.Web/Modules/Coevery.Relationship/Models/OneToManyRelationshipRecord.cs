using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models
{
    public class OneToManyRelationshipRecord : ContentPartRecord
    {
        public virtual int Relationship_Id { get; set; }
        public virtual int LookupField_Id { get; set; }
        public virtual string RelatedListLabel { get; set; }
        public virtual int ShowRelatedList { get; set; }//bit
        public virtual int DeleteOption { get; set; }//tinyint
    }
}