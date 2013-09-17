using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
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
        private static readonly Regex _dateRegEx = new Regex(@"(?<year>\d{1,4})(\-(?<month>\d{1,2})(\-(?<day>\d{1,2})\s*((?<hour>\d{1,2})(:(?<minute>\d{1,2})(:(?<second>\d{1,2}))?)?)?)?)?");

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

            string type = Convert.ToString(formState.ValueType);

            DateTime min, max;

            // Are those dates or time spans
            if (type == "0") {
                if (op == DateTimeOperator.Between || op == DateTimeOperator.NotBetween) {
                    min = GetLowBoundPattern(Convert.ToString(formState.Min));
                    max = GetHighBoundPattern(Convert.ToString(formState.Max));
                }
                else {
                    min = GetLowBoundPattern(Convert.ToString(formState.Value));
                    max = GetHighBoundPattern(Convert.ToString(formState.Value));
                }
            }
            else {
                if (op == DateTimeOperator.Between || op == DateTimeOperator.NotBetween) {
                    min = ApplyDelta(now, formState.MinUnit, Int32.Parse(formState.Min));
                    max = ApplyDelta(now, formState.MinUnit, Int32.Parse(formState.Min));
                }
                else {
                    min = max = ApplyDelta(now, Convert.ToString(formState.ValueUnit), Convert.ToInt32(formState.Value));
                }
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

        /// <summary>
        /// Returns the low bound value of a date pattern. e.g., 2011-10 will return 2011-10-01 00:00:00
        /// </summary>
        /// <remarks>DateTime is stored in UTC but entered in local</remarks>
        protected static DateTime GetLowBoundPattern(string datePattern) {
            var match = _dateRegEx.Match(datePattern);

            return DateTime.Parse(
                String.Format("{0}-{1}-{2} {3}:{4}:{5}",
                    match.Groups["year"].Success ? match.Groups["year"].Value : "1980",
                    match.Groups["month"].Success ? match.Groups["month"].Value : "01",
                    match.Groups["day"].Success ? match.Groups["day"].Value : "01",
                    match.Groups["hour"].Success ? match.Groups["hour"].Value : "00",
                    match.Groups["minute"].Success ? match.Groups["minute"].Value : "00",
                    match.Groups["second"].Success ? match.Groups["second"].Value : "00"),
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
        }

        protected static DateTime ApplyDelta(DateTime now, string unit, int value) {
            var span = (DateTimeSpan) Enum.Parse(typeof (DateTimeSpan), unit);

            switch (span) {
                case DateTimeSpan.Year:
                    return now.AddYears(value);
                case DateTimeSpan.Month:
                    return now.AddMonths(value);
                case DateTimeSpan.Day:
                    return now.AddDays(value);
                case DateTimeSpan.Hour:
                    return now.AddHours(value);
                case DateTimeSpan.Minute:
                    return now.AddMinutes(value);
                case DateTimeSpan.Second:
                    return now.AddSeconds(value);
                default:
                    return now;
            }
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            var op = (DateTimeOperator) Enum.Parse(typeof (DateTimeOperator), Convert.ToString(formState.Operator));
            string type = Convert.ToString(formState.ValueType);
            string value = Convert.ToString(formState.Value);
            string min = Convert.ToString(formState.Min);
            string max = Convert.ToString(formState.Max);
            string valueUnit = Convert.ToString(formState.ValueUnit);
            string minUnit = Convert.ToString(formState.MinUnit);
            string maxUnit = Convert.ToString(formState.MaxUnit);

            if (type == "0") {
                valueUnit = minUnit = maxUnit = String.Empty;
            }
            else {
                valueUnit = " " + valueUnit;
                minUnit = " " + minUnit;
                maxUnit = " " + maxUnit;
            }

            switch (op) {
                case DateTimeOperator.LessThan:
                    return T("{0} is less than {1}{2}", fieldName, value, T(valueUnit).Text);
                case DateTimeOperator.LessThanEquals:
                    return T("{0} is less or equal than {1}{2}", fieldName, value, T(valueUnit).Text);
                case DateTimeOperator.Equals:
                    return T("{0} equals {1}{2}", fieldName, value, T(valueUnit).Text);
                case DateTimeOperator.NotEquals:
                    return T("{0} is not equal to {1}{2}", fieldName, value, T(valueUnit).Text);
                case DateTimeOperator.GreaterThan:
                    return T("{0} is greater than {1}{2}", fieldName, value, T(valueUnit).Text);
                case DateTimeOperator.GreaterThanEquals:
                    return T("{0} is greater or equal than {1}{2}", fieldName, value, T(valueUnit).Text);
                case DateTimeOperator.Between:
                    return T("{0} is between {1}{2} and {3}{4}", fieldName, min, T(minUnit).Text, max, T(maxUnit).Text);
                case DateTimeOperator.NotBetween:
                    return T("{0} is not between {1}{2} and {3}{4}", fieldName, min, T(minUnit).Text, max, T(maxUnit).Text);
            }

            // should never be hit, but fail safe
            return new LocalizedString(fieldName);
        }

        /// <summary>
        /// Returns the low bound value of a date pattern. e.g., 2011-10 will return 2011-10-01 00:00:00
        /// </summary>
        /// <remarks>DateTime is stored in UTC but entered in local</remarks>
        protected static DateTime GetHighBoundPattern(string datePattern) {
            var match = _dateRegEx.Match(datePattern);

            string year, month;
            return DateTime.Parse(
                String.Format("{0}-{1}-{2} {3}:{4}:{5}",
                    year = match.Groups["year"].Success ? match.Groups["year"].Value : "2099",
                    month = match.Groups["month"].Success ? match.Groups["month"].Value : "12",
                    match.Groups["day"].Success ? match.Groups["day"].Value : DateTime.DaysInMonth(Int32.Parse(year), Int32.Parse(month)).ToString(),
                    match.Groups["hour"].Success ? match.Groups["hour"].Value : "23",
                    match.Groups["minute"].Success ? match.Groups["minute"].Value : "59",
                    match.Groups["second"].Success ? match.Groups["second"].Value : "59"),
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
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

    public enum DateTimeSpan {
        Year,
        Month,
        Day,
        Hour,
        Minute,
        Second
    }
}