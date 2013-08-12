using Orchard.ContentManagement.Records;
using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Relationship.Records
{
    public enum RelationshipType {
        OneToMany = 0,
        ManyToMany = 1
    }

    public class RelationshipRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual RelationshipType Type { get; set; }
        public virtual int PrimaryEntityId { get; set; }
        public virtual int RelatedEntityId { get; set; }
    }
}