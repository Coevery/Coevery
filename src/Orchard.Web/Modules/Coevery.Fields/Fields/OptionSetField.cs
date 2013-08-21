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

        public string[] OptionValue {
            get {
                if (string.IsNullOrWhiteSpace(Value)) {
                    return null;
                }
                return Value.Split(OptionSetConfig.LabelSeperator, StringSplitOptions.RemoveEmptyEntries);
            }
            set {
                if (value != null) {
                    Value = string.Join(OptionSetConfig.LabelSeperator[0], value);
                }
            }
        }

        public string Value
        {
            get { return Storage.Get<string>(Name); }
            set { Storage.Set(value ?? string.Empty); }
        }
    }
}
