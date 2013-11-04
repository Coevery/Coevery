using System;
using Coevery.Projections.Models;
using Coevery.Projections.FilterEditors.Forms;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Services;

namespace Coevery.Projections.FieldTypeEditors {
    public class DateFieldTypeEditor : ConcreteFieldTypeEditorBase {
        private readonly IClock _clock;

        public Localizer T { get; set; }

        public DateFieldTypeEditor(IClock clock) {
            _clock = clock;
            T = NullLocalizer.Instance;
        }

        public override bool CanHandle(string fieldTypeName, Type storageType) {
            return fieldTypeName == "DateField";
        }

        public override string FormName {
            get { return DateFilterForm.FormName; }
        }

        public override Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return DateFilterForm.GetFilterPredicate(formState, "Value", _clock.UtcNow, true);
        }

        public override LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return DateFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public override Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}