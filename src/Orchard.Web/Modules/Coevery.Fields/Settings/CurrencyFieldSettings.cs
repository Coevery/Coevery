namespace Coevery.Fields.Settings {
    public class CurrencyFieldSettings : FieldSettings {
        public uint Length { get; set; }
        public uint DecimalPlaces { get; set; }
        public decimal DefaultValue { get; set; }

        public CurrencyFieldSettings()
        {
            Length = 18;
            DecimalPlaces = 0;
        }
    }
}