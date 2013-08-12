using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Records
{
    public enum OneToManyDeleteOption {
        NoAction = 0,
        NotAllowed = 1,
        CascadingDelete = 2
    }

    public class OneToManyRelationshipRecord
    {
        public int Id { get; set; }
        public RelationshipRecord Relationship { get; set; }
        public int LookupFieldId { get; set; }
        public string RelatedListLabel { get; set; }
        public bool ShowRelatedList { get; set; }
        public OneToManyDeleteOption DeleteOption { get; set; }

        public OneToManyRelationshipRecord() {
            Relationship = new RelationshipRecord() {
                Type = RelationshipType.OneToMany
            };
        }
    }
}