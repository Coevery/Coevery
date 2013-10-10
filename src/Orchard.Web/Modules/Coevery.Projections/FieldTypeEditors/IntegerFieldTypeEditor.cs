using System;
using System.Linq;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Projections.Models;

namespace Coevery.Projections.FieldTypeEditors {
    [OrchardSuppressDependency("Orchard.Projections.FieldTypeEditors.IntegerFieldTypeEditor")]
    public class IntegerFieldTypeEditor : IFieldTypeEditor {
        public Localizer T { get; set; }

        public IntegerFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(Type storageType) {
            return new[] {
                typeof(Byte), 
                typeof(SByte), 
                typeof(Int16), 
                typeof(Int32), 
                typeof(Int64), 
                typeof(UInt16), 
                typeof(UInt32), 
                typeof(UInt64), 
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
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}