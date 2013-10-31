using Coevery.Entities.Models;

namespace Coevery.Relationship.Records {
    public enum RelationshipType {
        OneToMany = 0,
        ManyToMany = 1
    }

    public class RelationshipRecord {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual byte Type { get; set; }
        public virtual EntityMetadataRecord PrimaryEntity { get; set; }
        public virtual EntityMetadataRecord RelatedEntity { get; set; }
    }

    public interface IRelationshipRecord {
        RelationshipRecord Relationship { get; set; }
    }
}