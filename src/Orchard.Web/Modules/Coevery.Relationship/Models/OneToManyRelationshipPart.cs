using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Coevery.Relationship.Models
{
    public class OneToManyRelationshipPart : ContentPart<OneToManyRelationshipRecord>
    {
        [Required]
        public virtual int Relationship_Id
        {
            get { return Record.Relationship_Id; }
            set { Record.Relationship_Id = value; }
        }

        [Required]
        public virtual int LookupField_Id
        {
            get { return Record.LookupField_Id; }
            set { Record.LookupField_Id = value; }
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
        public virtual int DeleteOption
        {
            get { return Record.DeleteOption; }
            set { Record.DeleteOption = value; }
        }
    }
}