using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Coevery.Relationship.Models
{
    public class RelationshipColumnPart : ContentPart<RelationshipColumnRecord>
    {
        [Required]
        public virtual int Relationship_Id
        {
            get { return Record.Relationship_Id; }
            set { Record.Relationship_Id = value; }
        }

        [Required]
        public virtual int Column_Id
        {
            get { return Record.Column_Id; }
            set { Record.Column_Id = value; }
        }

        [Required]
        public virtual int IsRelatedList
        {
            get { return Record.IsRelatedList; }
            set { Record.IsRelatedList = value; }
        }
    }
}