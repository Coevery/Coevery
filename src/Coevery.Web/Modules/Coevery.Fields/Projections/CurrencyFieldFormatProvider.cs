using Coevery.ContentManagement;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;

namespace Coevery.Fields.Projections {
    public class CurrencyFieldFormatProvider : IContentFieldFormatProvider {

        public void SetFormat(ContentField field, dynamic formState) {
            var currencyField = field as CurrencyField;
            if (currencyField != null) {
                var settings = currencyField.PartFieldDefinition.Settings.GetModel<CurrencyFieldSettings>();
                if (formState.Format == null) {
                    formState.Format = "C" + settings.DecimalPlaces;
                }
            }
        }
    }
}