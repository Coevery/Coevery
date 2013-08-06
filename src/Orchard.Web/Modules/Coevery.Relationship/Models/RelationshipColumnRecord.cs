using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models
{
    public class RelationshipColumnRecord : ContentPartRecord
    {
        public virtual int Id { get; set; }
        public virtual RelationshipRecord Relationship { get; set; }
        public virtual int Column_Id { get; set; }
        public virtual bool IsRelatedList { get; set; }//bit
    }
}