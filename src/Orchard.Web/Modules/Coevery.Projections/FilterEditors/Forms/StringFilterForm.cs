using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace Coevery.Projections.FilterEditors.Forms {
    [OrchardSuppressDependency("Orchard.Projections.FilterEditors.Forms.StringFilterForm")]
    public class StringFilterForm : IFormProvider {
        public const string FormName = "StringFilter";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public StringFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {
                    var operators = new List<SelectListItem> {
                        new SelectListItem {Value = Convert.ToString(StringOperator.Equals), Text = T("Is equal to").Text},
                        new SelectListItem {Value = Convert.ToString(StringOperator.NotEquals), Text = T("Is not equal to").Text},
                        new SelectListItem {Value = Convert.ToString(StringOperator.Contains), Text = T("Contains").Text},
                        new SelectListItem {Value = Convert.ToString(StringOperator.ContainsAny), Text = T("Contains any word").Text},
                        new SelectListItem {Value = Convert.ToString(StringOperator.ContainsAll), Text = T("Contains all words").Text},
                        new SelectListItem {Value = Convert.ToString(StringOperator.Starts), Text = T("Starts with").Text},
                        new SelectListItem {Value = Convert.ToString(StringOperator.NotStarts), Text = T("Does not start with").Text},
                        new SelectListItem {Value = Convert.ToString(StringOperator.Ends), Text = T("Ends with").Text},
                        new SelectListItem {Value = Convert.ToString(StringOperator.NotEnds), Text = T("Does not end with").Text},
                        new SelectListItem {Value = Convert.ToString(StringOperator.NotContains), Text = T("Does not contain").Text}
                    };

                    var f = Shape.FilterEditors_StringFilter(
                        Id: FormName,
                        Operators: operators
                        );

                    return f;
                };

            context.Form(FormName, form);
        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property) {
            var op = (Orchard.Projections.FilterEditors.Forms.StringOperator) Enum.Parse(typeof (Orchard.Projections.FilterEditors.Forms.StringOperator), Convert.ToString(formState.Operator));
            object value = Convert.ToString(formState.Value);

            switch (op) {
                case Orchard.Projections.FilterEditors.Forms.StringOperator.Equals:
                    return x => x.Eq(property, value);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.NotEquals:
                    return x => x.Not(y => y.Eq(property, value));
                case Orchard.Projections.FilterEditors.Forms.StringOperator.Contains:
                    return x => x.Like(property, Convert.ToString(value), HqlMatchMode.Anywhere);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.ContainsAny:
                    var values1 = Convert.ToString(value).Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    var predicates1 = values1.Skip(1).Select<string, Action<IHqlExpressionFactory>>(x => y => y.Like(property, x, HqlMatchMode.Anywhere)).ToArray();
                    return x => x.Disjunction(y => y.Like(property, values1[0], HqlMatchMode.Anywhere), predicates1);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.ContainsAll:
                    var values2 = Convert.ToString(value).Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    var predicates2 = values2.Skip(1).Select<string, Action<IHqlExpressionFactory>>(x => y => y.Like(property, x, HqlMatchMode.Anywhere)).ToArray();
                    return x => x.Conjunction(y => y.Like(property, values2[0], HqlMatchMode.Anywhere), predicates2);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.Starts:
                    return x => x.Like(property, Convert.ToString(value), HqlMatchMode.Start);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.NotStarts:
                    return y => y.Not(x => x.Like(property, Convert.ToString(value), HqlMatchMode.Start));
                case Orchard.Projections.FilterEditors.Forms.StringOperator.Ends:
                    return x => x.Like(property, Convert.ToString(value), HqlMatchMode.End);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.NotEnds:
                    return y => y.Not(x => x.Like(property, Convert.ToString(value), HqlMatchMode.End));
                case Orchard.Projections.FilterEditors.Forms.StringOperator.NotContains:
                    return y => y.Not(x => x.Like(property, Convert.ToString(value), HqlMatchMode.Anywhere));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            var op = (Orchard.Projections.FilterEditors.Forms.StringOperator) Enum.Parse(typeof (Orchard.Projections.FilterEditors.Forms.StringOperator), Convert.ToString(formState.Operator));
            string value = Convert.ToString(formState.Value);
            fieldName = fieldName.Split('.')[1];
            switch (op) {
                case Orchard.Projections.FilterEditors.Forms.StringOperator.Equals:
                    return T("{0} is equal to '{1}'", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.NotEquals:
                    return T("{0} is not equal to '{1}'", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.Contains:
                    return T("{0} contains '{1}'", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.ContainsAny:
                    return T("{0} contains any of '{1}'", fieldName, new LocalizedString(String.Join("', '", value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))));
                case Orchard.Projections.FilterEditors.Forms.StringOperator.ContainsAll:
                    return T("{0} contains all '{1}'", fieldName, new LocalizedString(String.Join("', '", value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))));
                case Orchard.Projections.FilterEditors.Forms.StringOperator.Starts:
                    return T("{0} starts with '{1}'", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.NotStarts:
                    return T("{0} does not start with '{1}'", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.Ends:
                    return T("{0} ends with '{1}'", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.NotEnds:
                    return T("{0} does not end with '{1}'", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.StringOperator.NotContains:
                    return T("{0} does not contain '{1}'", fieldName, value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum StringOperator {
        Equals,
        NotEquals,
        Contains,
        ContainsAny,
        ContainsAll,
        Starts,
        NotStarts,
        Ends,
        NotEnds,
        NotContains,
    }
}