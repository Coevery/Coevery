using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models
{
    public class RelationshipRecord : ContentPartRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Type { get; set; }//tinyint
        public virtual int PrimaryEntity_Id { get; set; }
        public virtual int RelatedEntity_Id { get; set; }
    }
}