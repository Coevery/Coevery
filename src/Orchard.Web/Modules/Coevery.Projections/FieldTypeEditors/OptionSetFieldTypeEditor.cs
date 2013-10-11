using System;
using System.Linq;
using Coevery.OptionSet.Models;
using Coevery.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Models;

namespace Coevery.Projections.FieldTypeEditors {
    public class OptionSetFieldTypeEditor : ConcreteFieldTypeEditorBase {
        public Localizer T { get; set; }

        public OptionSetFieldTypeEditor() {
            T = NullLocalizer.Instance;
        }

        public override bool CanHandle(string fieldTypeName, Type storageType) {
            return fieldTypeName == "OptionSetField";
        }

        public override void ApplyFilter(FilterContext context, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field) {
            var op = (OptionSetOperator) Enum.Parse(typeof (OptionSetOperator), (string) context.State.Operator);
            string value = context.State.Value;
            var valueArr = value != null
                ? value.Split('&').Select(int.Parse).ToArray()
                : new[] {0};
            switch (op) {
                case OptionSetOperator.MatchesAny:
                    Action<IAliasFactory> selectorAny = alias => alias.ContentPartRecord<OptionItemContainerPartRecord>().Property("OptionItems", "opits").Property("OptionItemRecord", "opcpr");
                    Action<IHqlExpressionFactory> filterAny = x => x.InG("Id", valueArr);
                    context.Query.Where(selectorAny, filterAny);
                    break;
                case OptionSetOperator.MatchesAll:
                    foreach (var id in valueArr) {
                        var optionId = id;
                        Action<IAliasFactory> selectorAll =
                            alias => alias.ContentPartRecord<OptionItemContainerPartRecord>().Property("OptionItems", "opit" + optionId);
                        Action<IHqlExpressionFactory> filterAll = x => x.Eq("OptionItemRecord.Id", optionId);
                        context.Query.Where(selectorAll, filterAll);
                    }
                    break;
                case OptionSetOperator.NotMatchesAny:
                    Action<IAliasFactory> selectorNotAny = alias => alias.ContentPartRecord<OptionItemContainerPartRecord>().Property("OptionItems", "opits").Property("OptionItemRecord", "opcpr");
                    Action<IHqlExpressionFactory> filterNotAny = x => x.Not(y => y.InG("Id", valueArr));
                    context.Query.Where(selectorNotAny, filterNotAny);
                    break;
            }
        }

        public override void ApplySortCriterion(Orchard.Projections.Descriptors.SortCriterion.SortCriterionContext context, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field) {}

        public override string FormName {
            get { return OptionSetFilterForm.FormName; }
        }

        public override Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState) {
            return null;
        }

        public override LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState) {
            return OptionSetFilterForm.DisplayFilter(fieldName + " " + storageName, formState, T);
        }

        public override Action<IAliasFactory> GetFilterRelationship(string aliasName) {
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("StringFieldIndexRecords", aliasName);;
        }
    }
}