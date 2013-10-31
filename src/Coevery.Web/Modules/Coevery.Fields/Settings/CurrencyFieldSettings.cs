using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class CurrencyFieldSettings : FieldSettings {
        public int Length { get; set; }
        public int DecimalPlaces { get; set; }
        public decimal? DefaultValue { get; set; }

        public CurrencyFieldSettings()
        {
            Length = 18;
            DecimalPlaces = 0;
            
        }
    }
}