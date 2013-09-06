
using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class UrlFieldSettings : FieldSettings {
        public string DefaultValue { get; set; }

        public UrlFieldSettings()
        {
            DefaultValue = null;
        }
    }
}
