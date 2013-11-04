using Coevery.ContentManagement;
using Coevery.ContentManagement.FieldStorage;

namespace Coevery.Core.Common.Fields {
    public class TextField : ContentField {
        public string Value {
            get { return Storage.Get<string>(); }
            set { Storage.Set(value); }
        }
    }
}
