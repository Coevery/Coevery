using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models
{
    public class RelationshipColumnRecord : ContentPartRecord
    {
        public virtual int Relationship_Id { get; set; }
        public virtual int Column_Id { get; set; }
        public virtual int IsRelatedList { get; set; }//bit
    }
}