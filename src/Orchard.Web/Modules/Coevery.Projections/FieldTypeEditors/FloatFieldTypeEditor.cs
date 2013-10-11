using System;
using System.Linq;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Projections.Models;

namespace Coevery.Projections.FieldTypeEditors {
    [OrchardSuppressDependency("Orchard.Projections.FieldTypeEditors.FloatFieldTypeEditor")]
    public class FloatFieldTypeEditor : IFieldTypeEditor {
        public Localizer T { get; set; }

        public FloatFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(Type storageType) {
            return new[] {
                typeof(float?), 
                typeof(double?)
            }.Contains(storageType);
        }

        public string FormName {
            get { return NumericFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return NumericFilterForm.GetFilterPredicate(formState, "Value");
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return NumericFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("DoubleFieldIndexRecords", aliasName);
        }
    }
}