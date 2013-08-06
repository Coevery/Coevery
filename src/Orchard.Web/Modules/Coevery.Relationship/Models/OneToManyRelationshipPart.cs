using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Coevery.Relationship.Models
{
    public class OneToManyRelationshipPart : ContentPart<OneToManyRelationshipRecord>
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
        public virtual bool ShowRelatedList
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