using System;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Projections.Models;

namespace Coevery.Projections.FieldTypeEditors {
    public class ReferenceFieldTypeEditor : IConcreteFieldTypeEditor {
        public Localizer T { get; set; }
        public Filter Filter { get; set; }

        public ReferenceFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(string fieldTypeName, Type storageType) {
            return fieldTypeName == "ReferenceField";
        }

        public bool CanHandle(Type storageType) {
            return false;
        }

        public string FormName {
            get { return ReferenceFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return ReferenceFilterForm.GetFilterPredicate(formState, "Value");
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return ReferenceFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}