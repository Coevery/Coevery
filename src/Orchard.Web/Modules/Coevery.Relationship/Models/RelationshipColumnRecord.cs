using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models
{
    public class RelationshipColumnRecord
    {
        public virtual int Id { get; set; }
        public virtual RelationshipRecord Relationship { get; set; }
        public virtual int ColumnId { get; set; }
        public virtual bool IsRelatedList { get; set; }//bit
    }
}