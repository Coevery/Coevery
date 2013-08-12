using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Records
{
    public class ManyToManyRelationshipRecord
    {
        public int Id { get; set; }
        public RelationshipRecord Relationship { get; set; }
        public string RelatedListLabel { get; set; }
        public bool ShowRelatedList { get; set; }
        public string PrimaryListLabel { get; set; }
        public bool ShowPrimaryList { get; set; }

        public ManyToManyRelationshipRecord() {
            Relationship = new RelationshipRecord() {
                Type = RelationshipType.ManyToMany
            };
        }
    }
}