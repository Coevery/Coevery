using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;
using Orchard.ContentManagement;

namespace Coevery.Fields.Projections {
    public class DefaultFieldStateProvider : FieldToPropertyStateProvider {
        public DefaultFieldStateProvider() {
            FieldTypeSet = new[] {
                "EmailField", "CoeveryTextField","PhoneField","UrlField",
                 "BooleanField", "OptionSetField", "ReferenceField"
            };
        }
        public override string GetPropertyState(string fieldType, string filedName, IDictionary<string, string> customSettings) {
            return string.Format(Format, filedName, null);
        }
    }
}