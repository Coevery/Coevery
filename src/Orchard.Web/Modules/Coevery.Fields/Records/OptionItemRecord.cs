using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Fields.Records {
    public class OptionItemRecord {
        public virtual int Id { get; set; }
        public virtual ContentPartFieldDefinitionRecord ContentPartFieldDefinitionRecord { get; set; }
        public virtual string Value { get; set; }
        public virtual bool IsDefault { get; set; }
    }
}
