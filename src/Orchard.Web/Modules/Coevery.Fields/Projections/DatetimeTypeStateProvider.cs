using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;
using Orchard.ContentManagement;

namespace Coevery.Fields.Projections {
    public class DatetimeTypeStateProvider : FieldToPropertyStateProvider {
        public DatetimeTypeStateProvider() {
            FieldTypeSet = new[] { "DateField", "DatetimeField" };
        }
        public override string GetPropertyState(string fieldType, string filedName, IDictionary<string,string> customSettings) {
            string formatOption;
            switch (fieldType) {
                case "DateField":
                    formatOption = "d";
                    break;
                case "DatetimeField":
                    formatOption = "g";
                    break;
                default:
                    return null;
            }
            return string.Format(Format, filedName, formatOption);
        }
    }
}