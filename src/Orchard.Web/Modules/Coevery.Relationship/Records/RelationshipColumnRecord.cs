using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Records
{
    public class RelationshipColumnRecord
    {
        public virtual int Id { get; set; }
        public virtual int RelationshipId { get; set; }
        public virtual int ColumnId { get; set; }
        public virtual bool IsRelatedList { get; set; }
    }
}