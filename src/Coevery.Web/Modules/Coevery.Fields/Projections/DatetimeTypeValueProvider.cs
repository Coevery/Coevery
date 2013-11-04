using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Entities.Services;
using Coevery.Fields.Fields;
using Coevery.ContentManagement;

namespace Coevery.Fields.Projections {
    public class DateFieldValueProvider : ContentFieldValueProvider<DateField> {
        public override object GetValue(ContentItem contentItem, ContentField field) {
            var value = field.Storage.Get<DateTime?>(field.Name);
            if (!value.HasValue) {
                return null;
            }
            return value.Value.ToLocalTime();
        }
    }
    public class DatetimeFieldValueProvider : ContentFieldValueProvider<DatetimeField> {
        public override object GetValue(ContentItem contentItem, ContentField field) {
            var value = field.Storage.Get<DateTime?>(field.Name);
            if (!value.HasValue) {
                return null;
            }
            return value.Value.ToLocalTime();
        }
    }
}