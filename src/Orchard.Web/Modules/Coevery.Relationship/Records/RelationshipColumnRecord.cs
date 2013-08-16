
using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Relationship.Records
{
    public class RelationshipColumnRecord
    {
        public virtual int Id { get; set; }
        public virtual RelationshipRecord RelationshipRecord { get; set; }
        public virtual ContentPartFieldDefinitionRecord Column { get; set; }
        public virtual bool IsRelatedList { get; set; }
    }
}