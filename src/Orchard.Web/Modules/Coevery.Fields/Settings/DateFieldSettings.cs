using System;
using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class DateFieldSettings : FieldSettings {
        public DateTime? DefaultValue { get ; set; }

        public DateFieldSettings() {
            DefaultValue = null;
        }

    }
}
