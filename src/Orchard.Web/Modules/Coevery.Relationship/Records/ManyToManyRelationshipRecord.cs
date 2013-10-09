using Orchard.Projections.Models;

namespace Coevery.Relationship.Records {
    public class ManyToManyRelationshipRecord : IRelationshipRecord {
        public virtual int Id { get; set; }
        public virtual RelationshipRecord Relationship { get; set; }
        public virtual ProjectionPartRecord RelatedListProjection { get; set; }
        public virtual string RelatedListLabel { get; set; }
        public virtual bool ShowRelatedList { get; set; }
        public virtual ProjectionPartRecord PrimaryListProjection { get; set; }
        public virtual string PrimaryListLabel { get; set; }
        public virtual bool ShowPrimaryList { get; set; }
    }
}