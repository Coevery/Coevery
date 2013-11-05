using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;
using Coevery.ContentManagement;

namespace Coevery.Fields.Projections {
    public class DefaultFieldStateProvider : FieldToPropertyStateProvider {
        public DefaultFieldStateProvider() {
            FieldTypeSet = new[] {
                "EmailField", "TextField","PhoneField","UrlField",
                 "BooleanField", "OptionSetField", "ReferenceField"
            };
        }
        public override string GetPropertyState(string fieldType, string filedName, IDictionary<string, string> customSettings) {
            return string.Format(Format, filedName, null);
        }
    }
}