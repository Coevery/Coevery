using Orchard.Data.Conventions;

namespace Coevery.OptionSet.Models {
    /// <summary>
    /// Represents a relationship between a Term and a Content Item
    /// </summary>
    public class OptionItemContentItem {
        
        public virtual int Id { get; set; }
        public virtual string Field { get; set; }
        public virtual OptionItemPartRecord OptionItemRecord { get; set; }

        [CascadeAllDeleteOrphan]
        public virtual OptionItemContainerPartRecord OptionItemContainerPartRecord { get; set; }
    }
}