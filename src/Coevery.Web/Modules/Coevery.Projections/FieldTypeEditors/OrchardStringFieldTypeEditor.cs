using System;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Projections.FilterEditors.Forms;
using Coevery.Projections.Models;

namespace Coevery.Projections.FieldTypeEditors {
    /// <summary>
    /// <see cref="IFieldTypeEditor"/> implementation for string properties
    /// </summary>
    public class OrchardStringFieldTypeEditor : IFieldTypeEditor {
        public Localizer T { get; set; }

        public OrchardStringFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(Type storageType) {
            return new[] { typeof(string), typeof(char) }.Contains(storageType);
        }

        public string FormName {
            get { return OrchardStringFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return OrchardStringFilterForm.GetFilterPredicate(formState, "Value");
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return OrchardStringFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("StringFieldIndexRecords", aliasName);
        }
    }
}