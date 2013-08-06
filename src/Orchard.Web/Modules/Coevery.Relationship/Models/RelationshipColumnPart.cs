using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Coevery.Relationship.Models
{
    public class RelationshipColumnPart : ContentPart<RelationshipColumnRecord>
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
        public virtual int Column_Id
        {
            get { return Record.Column_Id; }
            set { Record.Column_Id = value; }
        }

        [Required]
        public virtual bool IsRelatedList
        {
            get { return Record.IsRelatedList; }
            set { Record.IsRelatedList = value; }
        }
    }
}