using Coevery.ContentManagement;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;

namespace Coevery.Fields.Projections {
    public class DateFieldFormatProvider : IContentFieldFormatProvider {

        public void SetFormat(ContentField field, dynamic formState) {
            var dateField = field as DateField;
            if (dateField != null) {
                if (formState.Format == null) {
                    formState.Format = "d";
                }
            }
        }
    }
}