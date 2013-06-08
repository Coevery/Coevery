namespace Coevery.Fields.Settings {
    public class BooleanFieldSettings : FieldSettings {
        public string OnLabel { get; set; }
        public string OffLabel { get; set; }
        public BooleanDisplayMode SelectionMode { get; set; }
        public bool DefaultValue { get; set; }

        public BooleanFieldSettings() {
            OnLabel = "Yes";
            OffLabel = "No";
            SelectionMode = BooleanDisplayMode.Checkbox;
        }
    }

    public enum BooleanDisplayMode {
        Checkbox,
        Radiobutton
    }
}
