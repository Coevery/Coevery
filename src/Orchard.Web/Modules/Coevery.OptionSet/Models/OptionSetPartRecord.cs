using Orchard.ContentManagement.Records;

namespace Coevery.OptionSet.Models {
    public class OptionSetPartRecord : ContentPartRecord {
        public virtual string TermTypeName { get; set; }
        public virtual bool IsInternal { get; set; }
    }
}
