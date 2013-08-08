using Orchard.ContentManagement.Records;
using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Relationship.Models
{
    public class RelationshipRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual byte Type { get; set; }//tinyint
        public virtual ContentTypeDefinitionRecord PrimaryEntity { get; set; }
        public virtual ContentTypeDefinitionRecord RelatedEntity { get; set; }
    }
}