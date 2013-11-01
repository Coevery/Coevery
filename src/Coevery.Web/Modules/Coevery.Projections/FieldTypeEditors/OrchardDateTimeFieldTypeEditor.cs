using System;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Projections.FilterEditors.Forms;
using Coevery.Projections.Models;
using Coevery.Services;

namespace Coevery.Projections.FieldTypeEditors {
    /// <summary>
    /// <see cref="IFieldTypeEditor"/> implementation for DateTime properties
    /// </summary>
    public class OrchardDateTimeFieldTypeEditor : IFieldTypeEditor {
        private readonly IClock _clock;
        
        public Localizer T { get; set; }

        public OrchardDateTimeFieldTypeEditor(IClock clock) {
            _clock = clock;
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(Type storageType) {
            return new[] { typeof(DateTime), typeof(DateTime?) }.Contains(storageType);
        }

        public string FormName {
            get { return OrchardDateTimeFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return OrchardDateTimeFilterForm.GetFilterPredicate(formState, "Value", _clock.UtcNow, true);
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return OrchardDateTimeFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}