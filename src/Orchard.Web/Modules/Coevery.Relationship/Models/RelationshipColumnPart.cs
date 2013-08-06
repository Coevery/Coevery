using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Coevery.Relationship.Models
{
    public class RelationshipColumnPart : ContentPart<RelationshipColumnRecord>
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
        public int Column_Id
        {
            get { return Record.Column_Id; }
            set { Record.Column_Id = value; }
        }

        [Required]
        public bool IsRelatedList
        {
            get { return Record.IsRelatedList; }
            set { Record.IsRelatedList = value; }
        }
    }
}