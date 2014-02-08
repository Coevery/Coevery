using System;
using System.Linq;
using Coevery.OptionSet.Models;
using Coevery.Projections.Models;
using Coevery.Projections.FilterEditors.Forms;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Localization;
using Coevery.Projections.Descriptors.Filter;
using Coevery.Utility.Extensions;

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
            string value = context.State.Value;
            if (string.IsNullOrWhiteSpace(value)) {
                return;
            }

            var op = (OptionSetOperator)Enum.Parse(typeof(OptionSetOperator), (string)context.State.Operator);
            var valueArr = value.Split('&').Select(int.Parse).ToArray();
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

        public override void ApplySortCriterion(Descriptors.SortCriterion.SortCriterionContext context, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field) {
            var ascending = (bool)context.State.Sort;
            var propertyName = String.Join(".", part.Name, field.Name, storageName ?? "");
            var safeName = propertyName.ToSafeName();
            Action<IHqlExpressionFactory> predicate = y => y.Eq("PropertyName", propertyName);
            Action<IAliasFactory> relationship = x => x.ContentPartRecord<FieldIndexPartRecord>().Property("StringFieldIndexRecords", safeName);
           
            context.Query = context.Query.Where(relationship, predicate);
            context.Query = ascending
                ? context.Query.OrderBy(relationship, x => x.Asc("Value"))
                : context.Query.OrderBy(relationship, x => x.Desc("Value"));
        }

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
            return x => x.ContentPartRecord<FieldIndexPartRecord>().Property("StringFieldIndexRecords", aliasName);
        }
    }
}