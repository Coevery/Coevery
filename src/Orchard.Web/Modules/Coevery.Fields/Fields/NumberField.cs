using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;

namespace Coevery.Fields.Fields
{
    public class NumberField : ContentField
    {
        public double? Value
        {
            get { return Storage.Get<double?>(); }

            set { Storage.Set(value ?? 0); }
        }
    }
}
