using System.Text.RegularExpressions;

namespace Coevery.Fields.Settings {
    public class PhoneFieldSettings : FieldSettings {
        public string DefaultValue { get; set; }

        public PhoneFieldSettings()
        {
            //At most two hosts' numbers,host place in brackets or before '-',whitespaces will be ignored.
            //To be used. PhoneStringPatern = @"[ (?: \(\d{1,9}\) ){0,2} | (?: \d{1,9}- ){1,2} ] \d{1,20}";
            DefaultValue = null;
        }
    }
}
