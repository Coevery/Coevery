using System;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Models;

namespace Coevery.Projections.FieldTypeEditors {
    public class OptionSetFieldTypeEditor : ILogicFieldTypeEditor {
        public Localizer T { get; set; }

        public OptionSetFieldTypeEditor()
        {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(string fieldTypeName) {
            return fieldTypeName == "OptionSetField";
        }

        public bool CanHandle(Type storageType) {
            return false;
        }

        public string FormName {
            get { return OptionSetFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState)
        {
            return OptionSetFilterForm.GetFilterPredicate(formState, "Value");
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState)
        {
            return OptionSetFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName)
        {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }
    }
}