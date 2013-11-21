using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class MultilineTextFieldSettings : FieldSettings {
        public int MaxLength { get; set; }
        public string PlaceHolderText { get; set; }
        public int RowNumber { get; set; }
        public bool IsUnique { get; set; }

        public MultilineTextFieldSettings() {
            RowNumber = 3;
            MaxLength = 255;
        }
    }
}
