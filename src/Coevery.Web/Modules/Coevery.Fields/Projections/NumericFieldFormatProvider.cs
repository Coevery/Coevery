using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;
using Coevery.ContentManagement;
using Coevery.Fields.Settings;

namespace Coevery.Fields.Projections {
    public class NumericFieldFormatProvider : IContentFieldFormatProvider {

        public void SetFormat(ContentField field, dynamic formState) {
            var numberField = field as NumberField;
            if (numberField != null) {
                var settings = numberField.PartFieldDefinition.Settings.GetModel<NumberFieldSettings>();
                if (formState.Format == null) {
                    formState.Format = "F" + settings.DecimalPlaces;
                }
            }
        }
    }
}