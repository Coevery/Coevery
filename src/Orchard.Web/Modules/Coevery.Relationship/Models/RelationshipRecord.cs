using Orchard.ContentManagement.Records;
using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Relationship.Models
{
    public class RelationshipRecord : ContentPartRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Type { get; set; }//tinyint
        public virtual ContentTypeDefinitionRecord PrimaryEntity_Id { get; set; }
        public virtual ContentTypeDefinitionRecord RelatedEntity_Id { get; set; }
    }
}