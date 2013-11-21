using Coevery.ContentManagement;
using Coevery.ContentManagement.FieldStorage;

namespace Coevery.Fields.Fields
{
    public class NumberField : ContentField
    {
        public double? Value
        {
            get { return Storage.Get<double?>(Name); }

            set { Storage.Set(value); }
        }
    }
}
