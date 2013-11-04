using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class BooleanFieldSettings : FieldSettings {
        public BooleanDisplayMode SelectionMode { get; set; }
        public DependentType DependencyMode { get; set; }
        public bool DefaultValue { get; set; }

        public BooleanFieldSettings() {
            SelectionMode = BooleanDisplayMode.Checkbox;
            DependencyMode = DependentType.None;
        }
    }

    public enum BooleanDisplayMode {
        Checkbox,
        Radiobutton
    }
}
