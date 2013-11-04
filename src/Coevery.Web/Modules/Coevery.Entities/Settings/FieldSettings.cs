namespace Coevery.Entities.Settings {
    public class FieldSettings {
        public string HelpText { get; set; }
        public bool Required { get; set; }
        public bool ReadOnly { get; set; }
        public bool AlwaysInLayout { get; set; }
        public bool IsSystemField { get; set; }
        public bool IsAudit { get; set; }
    }
}