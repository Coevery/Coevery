using System;
using Coevery.ContentManagement;
using Coevery.ContentManagement.FieldStorage;

namespace Coevery.Fields.Fields {
    public class DateField : ContentField {
        public DateTime? Value {
            get {
                var temp = Storage.Get<DateTime?>(Name);
                if (!temp.HasValue) {
                    return null;
                }
                return temp.Value.ToLocalTime();
            }
            set {
                if (value.HasValue) {
                    value = value.Value.ToUniversalTime();
                }
                Storage.Set(value);
            }
        }
    }
}
