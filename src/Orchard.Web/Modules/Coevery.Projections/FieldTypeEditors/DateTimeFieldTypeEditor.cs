using System;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Projections.Models;
using Orchard.Services;

namespace Coevery.Projections.FieldTypeEditors {
    public class DateTimeFieldTypeEditor : IConcreteFieldTypeEditor {
        private readonly IClock _clock;

        public Localizer T { get; set; }
        public Filter Filter { get; set; }

        public DateTimeFieldTypeEditor(IClock clock) {
            _clock = clock;
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(string fieldTypeName, Type storageType) {
            return fieldTypeName == "DatetimeField";
        }

        public bool CanHandle(Type storageType) {
            return false;
        }

        public string FormName {
            get { return DateTimeFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return DateTimeFilterForm.GetFilterPredicate(formState, "Value", _clock.UtcNow, true);
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return DateTimeFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}