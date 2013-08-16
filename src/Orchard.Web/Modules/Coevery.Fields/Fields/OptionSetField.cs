using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using Coevery.Fields.Settings;


namespace Coevery.Fields.Fields
{
    public class OptionSetField : ContentField
    {
        public List<SelectListItem> Items { get; set; }
        public string DisplayItems { get; set; }
        public string[] OptionValue { get; set; }
        public int Value
        {
            get { return Storage.Get<int>(Name); }
            set { Storage.Set(value); }
        }
    }
}
