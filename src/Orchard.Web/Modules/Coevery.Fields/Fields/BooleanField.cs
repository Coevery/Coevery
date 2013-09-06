using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;

namespace Coevery.Fields.Fields {
    public class BooleanField : ContentField {
        public bool? Value {
            get { return Storage.Get<bool?>(Name); }
            set { Storage.Set(value); }
        }
    }
}
