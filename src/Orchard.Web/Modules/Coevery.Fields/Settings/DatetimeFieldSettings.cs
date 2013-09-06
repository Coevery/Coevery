using System;
using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class DatetimeFieldSettings : FieldSettings {
        public DateTime? DefaultValue { get ; set; }

        public DatetimeFieldSettings() {
            DefaultValue = null;
        }

    }
}
