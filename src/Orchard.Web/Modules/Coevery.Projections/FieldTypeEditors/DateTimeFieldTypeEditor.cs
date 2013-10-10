using System;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Projections.Models;
using Orchard.Services;

namespace Coevery.Projections.FieldTypeEditors {
    public class DateTimeFieldTypeEditor : ConcreteFieldTypeEditorBase {
        private readonly IClock _clock;

        public Localizer T { get; set; }

        public DateTimeFieldTypeEditor(IClock clock) {
            _clock = clock;
            T = NullLocalizer.Instance;
        }

        public override bool CanHandle(string fieldTypeName, Type storageType) {
            return fieldTypeName == "DatetimeField";
        }

        public override string FormName {
            get { return DateTimeFilterForm.FormName; }
        }

        public override Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return DateTimeFilterForm.GetFilterPredicate(formState, "Value", _clock.UtcNow, true);
        }

        public override LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return DateTimeFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public override Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}