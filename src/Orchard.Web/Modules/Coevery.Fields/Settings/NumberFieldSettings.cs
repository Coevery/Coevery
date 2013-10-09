using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class NumberFieldSettings : FieldSettings {
        public uint Length { get; set; }
        public uint DecimalPlaces { get; set; }
        public double? DefaultValue { get; set; }

        public NumberFieldSettings()
        {
            Length = 18;
            DecimalPlaces = 0;
        }
    }
}