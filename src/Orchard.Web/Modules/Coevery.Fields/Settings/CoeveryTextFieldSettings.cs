namespace Coevery.Fields.Settings {
    public class CoeveryTextFieldSettings : FieldSettings {
        public int MaxLength { get; set; }
        public string PlaceHolderText { get; set; }

        public CoeveryTextFieldSettings() {
            MaxLength = 255;
        }
    }
}
