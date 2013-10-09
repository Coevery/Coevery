using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;
using Orchard.ContentManagement;

namespace Coevery.Fields.Projections {
    public class NumericTypeStateProvider : FieldToPropertyStateProvider {
        public NumericTypeStateProvider() {
            FieldTypeSet = new[] { "NumberField", "CurrencyField" };
        }
        public override string GetPropertyState(string fieldType, string filedName, IDictionary<string, string> customSettings) {
            string formatOption;
            switch (fieldType) {
                case "NumberField":
                    formatOption = "F" + customSettings[fieldType + "Settings." + "DecimalPlaces"];
                    break;
                case "CurrencyField":
                    formatOption = "C" + customSettings[fieldType + "Settings." + "DecimalPlaces"];
                    break;
                default:
                    return null;
            }
            return string.Format(Format, filedName, formatOption);
        }
    }
}