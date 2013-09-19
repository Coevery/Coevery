using System;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace Coevery.Projections.FieldTypeEditors {
    public class OptionSetFieldTypeEditor : ILogicFieldTypeEditor {
        public Localizer T { get; set; }

        public bool NeedApplyFilter {
            get { return true; }
        }

        public OptionSetFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public bool CanHandle(string fieldTypeName) {
            return fieldTypeName == "OptionSetField";
        }

        public void ApplyFilter(dynamic context) {
            var op = (OptionSetOperator) Enum.Parse(typeof (OptionSetOperator), (string) context.State.Operator);
            string value = context.State.Value;
            string[] valueArrStr = value.Split(new string[] {"&"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool CanHandle(Type storageType) {
            return false;
        }

        public string FormName {
            get { return OptionSetFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return null;
        }

        public LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return OptionSetFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return null;
        }
    }
}