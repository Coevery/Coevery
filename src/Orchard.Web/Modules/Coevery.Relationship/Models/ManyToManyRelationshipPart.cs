using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Coevery.Relationship.Models
{
    public class ManyToManyRelationshipPart : ContentPart<ManyToManyRelationshipRecord>
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
        public string PrimaryListLabel
        {
            get { return Record.PrimaryListLabel; }
            set { Record.PrimaryListLabel = value; }
        }

        [Required]
        public bool ShowPrimaryList
        {
            get { return Record.ShowPrimaryList; }
            set { Record.ShowPrimaryList = value; }
        }
    }
}