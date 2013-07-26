using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using Coevery.Fields.Settings;


namespace Coevery.Fields.Fields {
    public class SelectField : ContentField {
        public IEnumerable<SelectListItem> Items { get; set; }
        public int[] OptionId {
            get { 
                return Array.ConvertAll(Value.Split(SelectFieldSettings.LabelSeperator, StringSplitOptions.RemoveEmptyEntries), int.Parse); 
            }
        }
        public string Value {
            get { return Storage.Get<string>(Name); }
            set { Storage.Set(value); }
        }
    }
}
