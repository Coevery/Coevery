using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class NumberFieldSettings : FieldSettings {
        public int Length { get; set; }
        public int DecimalPlaces { get; set; }
        public double? DefaultValue { get; set; }

        public NumberFieldSettings() {
            Length = 18;
            DecimalPlaces = 0;
        }
    }
}