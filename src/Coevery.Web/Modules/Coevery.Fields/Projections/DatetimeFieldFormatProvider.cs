using Coevery.ContentManagement;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;

namespace Coevery.Fields.Projections {
    public class DatetimeFieldFormatProvider : IContentFieldFormatProvider {

        public void SetFormat(ContentField field, dynamic formState) {
            var datetimeField = field as DatetimeField;
            if (datetimeField != null) {
                if (formState.Format == null) {
                    formState.Format = "g";
                }
            }
        }
    }
}