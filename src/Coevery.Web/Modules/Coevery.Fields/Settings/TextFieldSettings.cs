using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class TextFieldSettings : FieldSettings {
        public int MaxLength { get; set; }
        public string PlaceHolderText { get; set; }
        public bool IsDispalyField { get; set; }

        public TextFieldSettings() {
            MaxLength = 255;
        }
    }
}
