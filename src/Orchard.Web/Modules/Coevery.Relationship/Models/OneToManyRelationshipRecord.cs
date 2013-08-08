using Orchard.ContentManagement.Records;
using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Relationship.Models
{
    public class OneToManyRelationshipRecord
    {
        public virtual int Id { get; set; }
        public virtual RelationshipRecord Relationship { get; set; }
        public virtual ContentTypeDefinitionRecord LookupFieldId { get; set; }
        public virtual string RelatedListLabel { get; set; }
        public virtual bool ShowRelatedList { get; set; }//bit
        public virtual int DeleteOption { get; set; }//tinyint
    }
}