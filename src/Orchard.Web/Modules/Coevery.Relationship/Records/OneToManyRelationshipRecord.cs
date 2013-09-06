using Orchard.Core.Settings.Metadata.Records;
using Orchard.Projections.Models;

namespace Coevery.Relationship.Records {
    public enum OneToManyDeleteOption {
        NoAction = 0,
        NotAllowed = 1,
        CascadingDelete = 2
    }

    public class OneToManyRelationshipRecord {
        public virtual int Id { get; set; }
        public virtual RelationshipRecord Relationship { get; set; }
        public virtual ContentPartFieldDefinitionRecord LookupField { get; set; }
        public virtual ProjectionPartRecord RelatedListProjection { get; set; }
        public virtual string RelatedListLabel { get; set; }
        public virtual bool ShowRelatedList { get; set; }
        public virtual byte DeleteOption { get; set; }
    }
}