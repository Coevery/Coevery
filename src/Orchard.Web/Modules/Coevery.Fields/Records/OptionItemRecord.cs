using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Fields.Records {
    public class OptionItemRecord {
        public virtual int Id { get; set; }
        public virtual string Value { get; set; }
        public virtual bool IsDefault { get; set; }
        public virtual OptionSetRecord OptionSetRecord { get; set; }
    }
}
