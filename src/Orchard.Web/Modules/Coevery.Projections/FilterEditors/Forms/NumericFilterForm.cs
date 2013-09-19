using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace Coevery.Projections.FilterEditors.Forms {
    [OrchardSuppressDependency("Orchard.Projections.FilterEditors.Forms.NumericFilterForm")]
    public class NumericFilterForm : IFormProvider {
        public const string FormName = "NumericFilter";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public NumericFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {
                    var operators = new List<SelectListItem> {
                        new SelectListItem {Value = Convert.ToString(NumericOperator.LessThan), Text = T("Is less than").Text},
                        new SelectListItem {Value = Convert.ToString(NumericOperator.LessThanEquals), Text = T("Is less than or equal to").Text},
                        new SelectListItem {Value = Convert.ToString(NumericOperator.Equals), Text = T("Is equal to").Text},
                        new SelectListItem {Value = Convert.ToString(NumericOperator.NotEquals), Text = T("Is not equal to").Text},
                        new SelectListItem {Value = Convert.ToString(NumericOperator.GreaterThanEquals), Text = T("Is greater than or equal to").Text},
                        new SelectListItem {Value = Convert.ToString(NumericOperator.GreaterThan), Text = T("Is greater than").Text},
                        new SelectListItem {Value = Convert.ToString(NumericOperator.Between), Text = T("Is between").Text},
                        new SelectListItem {Value = Convert.ToString(NumericOperator.NotBetween), Text = T("Is not between").Text}
                    };

                    var f = Shape.FilterEditors_NumericFilter(
                        Id: FormName,
                        Operators: operators
                        );

                    return f;
                };

            context.Form(FormName, form);
        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property) {
            var op = (Orchard.Projections.FilterEditors.Forms.NumericOperator) Enum.Parse(typeof (Orchard.Projections.FilterEditors.Forms.NumericOperator), Convert.ToString(formState.Operator));

            decimal min, max;

            if (op == Orchard.Projections.FilterEditors.Forms.NumericOperator.Between || op == Orchard.Projections.FilterEditors.Forms.NumericOperator.NotBetween) {
                min = Decimal.Parse(Convert.ToString(formState.Min), CultureInfo.InvariantCulture);
                max = Decimal.Parse(Convert.ToString(formState.Max), CultureInfo.InvariantCulture);
            }
            else {
                min = max = Decimal.Parse(Convert.ToString(formState.Value), CultureInfo.InvariantCulture);
            }

            switch (op) {
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.LessThan:
                    return x => x.Lt(property, max);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.LessThanEquals:
                    return x => x.Le(property, max);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.Equals:
                    if (min == max) {
                        return x => x.Eq(property, min);
                    }
                    return y => y.And(x => x.Ge(property, min), x => x.Le(property, max));
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.NotEquals:
                    return min == max ? (Action<IHqlExpressionFactory>) (x => x.Not(y => y.Eq(property, min))) : (y => y.Or(x => x.Lt(property, min), x => x.Gt(property, max)));
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.GreaterThan:
                    return x => x.Gt(property, min);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.GreaterThanEquals:
                    return x => x.Ge(property, min);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.Between:
                    return y => y.And(x => x.Ge(property, min), x => x.Le(property, max));
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.NotBetween:
                    return y => y.Or(x => x.Lt(property, min), x => x.Gt(property, max));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            var op = (Orchard.Projections.FilterEditors.Forms.NumericOperator) Enum.Parse(typeof (Orchard.Projections.FilterEditors.Forms.NumericOperator), Convert.ToString(formState.Operator));
            string value = Convert.ToString(formState.Value);
            string min = Convert.ToString(formState.Min);
            string max = Convert.ToString(formState.Max);
            fieldName = fieldName.Split('.')[1];

            switch (op) {
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.LessThan:
                    return T("{0} is less than {1}", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.LessThanEquals:
                    return T("{0} is less or equal than {1}", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.Equals:
                    return T("{0} equals {1}", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.NotEquals:
                    return T("{0} is not equal to {1}", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.GreaterThan:
                    return T("{0} is greater than {1}", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.GreaterThanEquals:
                    return T("{0} is greater or equal than {1}", fieldName, value);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.Between:
                    return T("{0} is between {1} and {2}", fieldName, min, max);
                case Orchard.Projections.FilterEditors.Forms.NumericOperator.NotBetween:
                    return T("{0} is not between {1} and {2}", fieldName, min, max);
            }

            // should never be hit, but fail safe
            return new LocalizedString(fieldName);
        }
    }

    public enum NumericOperator {
        LessThan,
        LessThanEquals,
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanEquals,
        Between,
        NotBetween
    }
}