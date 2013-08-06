using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Coevery.Relationship.Models
{
    public class ManyToManyRelationshipPart : ContentPart<ManyToManyRelationshipRecord>
    {
        [Required]
        public virtual int Relationship_Id
        {
            get { return Record.Relationship_Id; }
            set { Record.Relationship_Id = value; }
        }

        [Required]
        public virtual string RelatedListLabel
        {
            get { return Record.RelatedListLabel; }
            set { Record.RelatedListLabel = value; }
        }

        [Required]
        public virtual int ShowRelatedList
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
        public virtual int ShowPrimaryList
        {
            get { return Record.ShowPrimaryList; }
            set { Record.ShowPrimaryList = value; }
        }
    }
}