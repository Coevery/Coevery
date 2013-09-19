using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace Coevery.Projections.FilterEditors.Forms
{
    public class OptionSetFilterForm : IFormProvider
    {
        public const string FormName = "OptionSetFilter";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public OptionSetFilterForm(IShapeFactory shapeFactory)
        {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context)
        {
            Func<IShapeFactory, object> form =
                shape =>
                {
                    var operators = new List<SelectListItem> {
                        new SelectListItem {Value = Convert.ToString(OptionSetOperator.MatchesAny), Text = T("Maches any").Text},
                        new SelectListItem {Value = Convert.ToString(OptionSetOperator.MatchesAll), Text = T("Maches all").Text},
                        new SelectListItem {Value = Convert.ToString(OptionSetOperator.NotMatchesAny), Text = T("Not mathes any").Text}
                    };

                    var f = Shape.FilterEditors_OptionSetFilter(
                        Id: FormName,
                        Operators: operators
                        );
                    return f;
                };

            context.Form(FormName, form);
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T)
        {
            var op = (OptionSetOperator)Enum.Parse(typeof(OptionSetOperator), Convert.ToString(formState.Operator));
            switch (op)
            {
                case OptionSetOperator.MatchesAny:
                    return T("{0} matches any to '{1}'", fieldName, "");
                case OptionSetOperator.MatchesAll:
                    return T("{0} is matches all '{1}'", fieldName, "");
                case OptionSetOperator.NotMatchesAny:
                    return T("{0} not matches any '{1}'", fieldName, "");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property)
        {

            var op = (OptionSetOperator)Enum.Parse(typeof(OptionSetOperator), Convert.ToString(formState.Operator));
            string value = Convert.ToString(formState.Value);
            string[] valueArrStr = value.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            List<int> valueArr = new List<int>();
            foreach (var item in valueArrStr)
            {
                valueArr.Add(int.Parse(item));
            }
            switch (op)
            {
                case OptionSetOperator.MatchesAny:
                    return x => x.In(property, valueArr);
                case OptionSetOperator.MatchesAll:
                    return x => x.In(property, valueArr);
                case OptionSetOperator.NotMatchesAny:
                    return x => x.In(property, valueArr);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum OptionSetOperator
    {
        MatchesAny,
        MatchesAll,
        NotMatchesAny
    }
}