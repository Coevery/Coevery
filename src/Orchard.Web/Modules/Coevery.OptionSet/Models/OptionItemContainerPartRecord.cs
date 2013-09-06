using System.Collections.Generic;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Coevery.OptionSet.Models {
    public class OptionItemContainerPartRecord : ContentPartRecord {
        public OptionItemContainerPartRecord() {
            OptionItems = new List<OptionItemContentItem>();
        }

        [CascadeAllDeleteOrphan]
        public virtual IList<OptionItemContentItem> OptionItems { get; set; }
    }
}