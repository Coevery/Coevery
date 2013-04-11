using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;

namespace Coevery.Fields.Fields {
    public class DateTimeField : ContentField {

        public DateTime DateTime {
            get {
                var value = Storage.Get<DateTime>();
                return value;
            }

            set { Storage.Set(value); }
        }
    }
}
