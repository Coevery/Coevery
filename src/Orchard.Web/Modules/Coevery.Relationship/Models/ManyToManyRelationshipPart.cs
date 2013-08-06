using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Coevery.Relationship.Models
{
    public class ManyToManyRelationshipPart : ContentPart<ManyToManyRelationshipRecord>
    {
        [Required]
        public virtual int Id
        {
            get { return Record.Id; }
            set { Record.Id = value; }
        }

        [Required]
        public virtual RelationshipRecord Relationship
        {
            get { return Record.Relationship; }
            set { Record.Relationship = value; }
        }

        [Required]
        public virtual string RelatedListLabel
        {
            get { return Record.RelatedListLabel; }
            set { Record.RelatedListLabel = value; }
        }

        [Required]
        public virtual bool ShowRelatedList
        {
            get { return Record.ShowRelatedList; }
            set { Record.ShowRelatedList = value; }
        }

        [Required]
        public virtual string PrimaryListLabel
        {
            get { return Record.PrimaryListLabel; }
            set { Record.PrimaryListLabel = value; }
        }

        [Required]
        public virtual bool ShowPrimaryList
        {
            get { return Record.ShowPrimaryList; }
            set { Record.ShowPrimaryList = value; }
        }
    }
}