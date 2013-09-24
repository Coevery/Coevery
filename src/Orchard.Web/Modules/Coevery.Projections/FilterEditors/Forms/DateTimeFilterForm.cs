using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace Coevery.Projections.FilterEditors.Forms {
    [OrchardSuppressDependency("Orchard.Projections.FilterEditors.Forms.DateTimeFilterForm")]
    public class DateTimeFilterForm : IFormProvider {
        public const string FormName = "DateTimeFilter";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public DateTimeFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {
                    var operators = new List<SelectListItem> {
                        new SelectListItem {Value = Convert.ToString(DateTimeOperator.LessThan), Text = T("Is less than").Text},
                        new SelectListItem {Value = Convert.ToString(DateTimeOperator.LessThanEquals), Text = T("Is less than or equal to").Text},
                        new SelectListItem {Value = Convert.ToString(DateTimeOperator.Equals), Text = T("Is equal to").Text},
                        new SelectListItem {Value = Convert.ToString(DateTimeOperator.NotEquals), Text = T("Is not equal to").Text},
                        new SelectListItem {Value = Convert.ToString(DateTimeOperator.GreaterThanEquals), Text = T("Is greater than or equal to").Text},
                        new SelectListItem {Value = Convert.ToString(DateTimeOperator.GreaterThan), Text = T("Is greater than").Text},
                        new SelectListItem {Value = Convert.ToString(DateTimeOperator.Between), Text = T("Is between").Text},
                        new SelectListItem {Value = Convert.ToString(DateTimeOperator.NotBetween), Text = T("Is not between").Text}
                    };

                    var f = Shape.FilterEditors_DateTimeFilter(
                        Id: FormName,
                        Operators: operators
                        );
                    return f;
                };

            context.Form(FormName, form);
        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property, DateTime now, bool asTicks = false) {
            var op = (DateTimeOperator) Enum.Parse(typeof (DateTimeOperator), Convert.ToString(formState.Operator));
            DateTime min, max;

            // Are those dates or time spans
            if (op == DateTimeOperator.Between || op == DateTimeOperator.NotBetween) {
                min = DateTime.Parse((string) formState.Min);
                max = DateTime.Parse((string) formState.Max);
            }
            else {
                min = max = DateTime.Parse((string) formState.Value);
            }
            min = min.ToUniversalTime();
            max = max.ToUniversalTime();

            object minValue = min;
            object maxValue = max;

            if (asTicks) {
                minValue = min.Ticks;
                maxValue = max.Ticks;
            }

            switch (op) {
                case DateTimeOperator.LessThan:
                    return x => x.Lt(property, maxValue);
                case DateTimeOperator.LessThanEquals:
                    return x => x.Le(property, maxValue);
                case DateTimeOperator.Equals:
                    if (min == max) {
                        return x => x.Eq(property, minValue);
                    }
                    return y => y.And(x => x.Ge(property, minValue), x => x.Le(property, maxValue));
                case DateTimeOperator.NotEquals:
                    if (min == max) {
                        return x => x.Not(y => y.Eq(property, minValue));
                    }
                    return y => y.Or(x => x.Lt(property, minValue), x => x.Gt(property, maxValue));
                case DateTimeOperator.GreaterThan:
                    return x => x.Gt(property, minValue);
                case DateTimeOperator.GreaterThanEquals:
                    return x => x.Ge(property, minValue);
                case DateTimeOperator.Between:
                    return y => y.And(x => x.Ge(property, minValue), x => x.Le(property, maxValue));
                case DateTimeOperator.NotBetween:
                    return y => y.Or(x => x.Lt(property, minValue), x => x.Gt(property, maxValue));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            var op = (DateTimeOperator)Enum.Parse(typeof(DateTimeOperator), Convert.ToString(formState.Operator));
            string value = Convert.ToString(formState.Value);
            string min = Convert.ToString(formState.Min);
            string max = Convert.ToString(formState.Max);
            fieldName = fieldName.Split('.')[1];

            switch (op) {
                case DateTimeOperator.LessThan:
                    return T("{0} is less than {1}", fieldName, value);
                case DateTimeOperator.LessThanEquals:
                    return T("{0} is less or equal than {1}", fieldName, value);
                case DateTimeOperator.Equals:
                    return T("{0} equals {1}", fieldName, value);
                case DateTimeOperator.NotEquals:
                    return T("{0} is not equal to {1}", fieldName, value);
                case DateTimeOperator.GreaterThan:
                    return T("{0} is greater than {1}", fieldName, value);
                case DateTimeOperator.GreaterThanEquals:
                    return T("{0} is greater or equal than {1}", fieldName, value);
                case DateTimeOperator.Between:
                    return T("{0} is between {1} and {2}", fieldName, min, max);
                case DateTimeOperator.NotBetween:
                    return T("{0} is not between {1} and {2}", fieldName, min, max);
            }

            // should never be hit, but fail safe
            return new LocalizedString(fieldName);
        }
    }

    public enum DateTimeOperator {
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