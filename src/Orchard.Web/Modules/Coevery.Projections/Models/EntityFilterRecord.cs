using Orchard.Projections.Models;

namespace Coevery.Projections.Models {
    public class EntityFilterRecord {
        public virtual int Id { get; set; }
        public virtual string EntityName { get; set; }
        public virtual string Title { get; set; }
        public virtual FilterGroupRecord FilterGroupRecord { get; set; }
    }
}