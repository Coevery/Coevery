using System;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Projections.FilterEditors.Forms;
using Coevery.Projections.Models;

namespace Coevery.Projections.FieldTypeEditors {
    /// <summary>
    /// <see cref="IFieldTypeEditor"/> implementation for decimal properties
    /// </summary>
    public class OrchardDecimalFieldTypeEditor : IFieldTypeEditor {
        public Localizer T { get; set; }

        public OrchardDecimalFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(Type storageType) {
            return new[] {
                typeof(decimal), 
            }.Contains(storageType);
        }

        public string FormName {
            get { return OrchardNumericFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return OrchardNumericFilterForm.GetFilterPredicate(formState, "Value");
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return OrchardNumericFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("DecimalFieldIndexRecords", aliasName);
        }
    }
}