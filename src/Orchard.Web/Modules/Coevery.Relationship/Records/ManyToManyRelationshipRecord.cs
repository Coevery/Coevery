using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Relationship.Records
{
    public class ManyToManyRelationshipRecord
    {
        public virtual int Id { get; set; }
        public virtual RelationshipRecord Relationship { get; set; }
        public virtual string RelatedListLabel { get; set; }
        public virtual bool ShowRelatedList { get; set; }
        public virtual string PrimaryListLabel { get; set; }
        public virtual bool ShowPrimaryList { get; set; }
    }
}