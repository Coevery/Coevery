using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Relationship.Models
{
    public class RelationshipPart : ContentPart<RelationshipRecord>
    {
        [Required]
        public virtual int Id
        {
            get { return Record.Id; }
            set { Record.Id = value; }
        }

        [Required]
        public virtual string Name
        {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        [Required]
        public virtual int Type
        {
            get { return Record.Type; }
            set { Record.Type = value; }
        }

        [Required]
        public virtual ContentTypeDefinitionRecord PrimaryEntity_Id
        {
            get { return Record.PrimaryEntity_Id; }
            set { Record.PrimaryEntity_Id = value; }
        }

        [Required]
        public virtual ContentTypeDefinitionRecord RelatedEntity_Id
        {
            get { return Record.RelatedEntity_Id; }
            set { Record.RelatedEntity_Id = value; }
        }
    }
}