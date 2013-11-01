using System;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Projections.FilterEditors.Forms;
using Coevery.Projections.Models;

namespace Coevery.Projections.FieldTypeEditors {
    /// <summary>
    /// <see cref="IFieldTypeEditor"/> implementation for boolean properties
    /// </summary>
    public class OrchardBooleanFieldTypeEditor : IFieldTypeEditor {
        public Localizer T { get; set; }

        public OrchardBooleanFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(Type storageType) {
            return new[] { typeof(bool), typeof(bool?) }.Contains(storageType);
        }

        public string FormName {
            get { return OrchardBooleanFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return OrchardBooleanFilterForm.GetFilterPredicate(formState, "Value");
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return OrchardBooleanFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}