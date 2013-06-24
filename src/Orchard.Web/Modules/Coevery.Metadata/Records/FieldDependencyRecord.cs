using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Metadata.Records {
    public class FieldDependencyRecord {
        public virtual int Id { get; set; }
        public virtual ContentPartDefinitionRecord Entity { get; set; }
        public virtual ContentPartFieldDefinitionRecord ControlField { get; set; }
        public virtual ContentPartFieldDefinitionRecord DependentField { get; set; }
        public virtual string ControlFieldValue { get; set; }
        public virtual string DependentFieldValue { get; set; }
    }
}
