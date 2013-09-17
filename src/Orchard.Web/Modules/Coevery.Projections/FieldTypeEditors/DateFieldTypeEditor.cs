using System;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.Services;

namespace Coevery.Projections.FieldTypeEditors {
    public class DateFieldTypeEditor : ILogicFieldTypeEditor {
        private readonly IClock _clock;
        
        public Localizer T { get; set; }

        public DateFieldTypeEditor(IClock clock) {
            _clock = clock;
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(string fieldTypeName) {
            return fieldTypeName == "DateField";
        }

        public bool CanHandle(Type storageType) {
            return false;
        }

        public string FormName {
            get { return DateFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return DateFilterForm.GetFilterPredicate(formState, "Value", _clock.UtcNow, true);
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return DateFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}