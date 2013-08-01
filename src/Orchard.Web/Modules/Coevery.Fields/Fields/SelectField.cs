using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using Coevery.Fields.Settings;


namespace Coevery.Fields.Fields
{
    public class SelectField : ContentField
    {
        public List<SelectListItem> Items { get; set; }
        public string[] OptionValue
        {
            get
            {
                return string.IsNullOrWhiteSpace(Value) ? 
                    null: Value.Split(SelectFieldSettings.LabelSeperator, StringSplitOptions.RemoveEmptyEntries);
            }
            set
            {
                var tempValue = new StringBuilder();
                foreach (var label in value) {
                    tempValue.Append(label + SelectFieldSettings.LabelSeperator[0]);
                }
                Value = tempValue.ToString();
            }
        }
        public string Value
        {
            get { return Storage.Get<string>(Name); }
            set { Storage.Set(value ?? String.Empty); }
        }
    }
}
