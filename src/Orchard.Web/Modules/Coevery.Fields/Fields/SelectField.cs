using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;

namespace Coevery.Fields.Fields {
    public class SelectField : ContentField {

        public string Value {
            get { return Storage.Get<string>(Name); }
            set { Storage.Set(value); }
        }
    }
}
