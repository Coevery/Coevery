using Coevery.Data.Conventions;

namespace Coevery.ContentManagement.Records {
    public abstract class ContentPartRecord {
        public virtual int Id { get; set; }
        [CascadeAllDeleteOrphan]
        public virtual ContentItemRecord ContentItemRecord { get; set; }
    }
}
