using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Relationship.Models
{
    public class OneToManyRelationshipPart : ContentPart<OneToManyRelationshipRecord>
    {
        [Required]
        public int Id
        {
            get { return Record.Id; }
            set { Record.Id = value; }
        }

        [Required]
        public RelationshipRecord Relationship
        {
            get { return Record.Relationship; }
            set { Record.Relationship = value; }
        }

        [Required]
        public ContentTypeDefinitionRecord LookupField_Id
        {
            get { return Record.LookupField_Id; }
            set { Record.LookupField_Id = value; }
        }

        [Required]
        public string RelatedListLabel
        {
            get { return Record.RelatedListLabel; }
            set { Record.RelatedListLabel = value; }
        }

        [Required]
        public bool ShowRelatedList
        {
            get { return Record.ShowRelatedList; }
            set { Record.ShowRelatedList = value; }
        }

        [Required]
        public int DeleteOption
        {
            get { return Record.DeleteOption; }
            set { Record.DeleteOption = value; }
        }
    }
}