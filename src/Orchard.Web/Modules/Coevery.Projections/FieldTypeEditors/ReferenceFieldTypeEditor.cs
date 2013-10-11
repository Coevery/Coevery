using System;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.Utility.Extensions;

namespace Coevery.Projections.FieldTypeEditors {
    public class ReferenceFieldTypeEditor : ConcreteFieldTypeEditorBase {
        public Localizer T { get; set; }

        public ReferenceFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public override bool CanHandle(string fieldTypeName, Type storageType) {
            return fieldTypeName == "ReferenceField";
        }

        public override string FormName {
            get { return ReferenceFilterForm.FormName; }
        }

        public override Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return ReferenceFilterForm.GetFilterPredicate(formState, "Value");
        }

        public override LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return ReferenceFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public override Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("IntegerFieldIndexRecords", aliasName);
        }

        //public override void ApplySortCriterion(Orchard.Projections.Descriptors.SortCriterion.SortCriterionContext context, string storageName, Type storageType, Orchard.ContentManagement.MetaData.Models.ContentPartDefinition part, Orchard.ContentManagement.MetaData.Models.ContentPartFieldDefinition field) {
        //}
    }
}