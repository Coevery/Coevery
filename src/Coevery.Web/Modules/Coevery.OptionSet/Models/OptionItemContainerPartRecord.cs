using System.Collections.Generic;
using Coevery.ContentManagement.Records;
using Coevery.Data.Conventions;

namespace Coevery.OptionSet.Models {
    public class OptionItemContainerPartRecord : ContentPartVersionRecord {
        public OptionItemContainerPartRecord() {
            OptionItems = new List<OptionItemContentItem>();
        }

        [CascadeAllDeleteOrphan]
        public virtual IList<OptionItemContentItem> OptionItems { get; set; }
    }
}