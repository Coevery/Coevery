using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.ContentManagement;
using Coevery.DisplayManagement;
using Coevery.Forms.Services;
using Coevery.Localization;

namespace Coevery.Projections.FilterEditors.Forms {
    public class ReferenceFilterForm : IFormProvider {
        public const string FormName = "ReferenceFilter";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ReferenceFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {
                    var operators = new List<SelectListItem> {
                        new SelectListItem {Value = Convert.ToString(ReferenceOperator.MatchesAny), Text = T("Maches any").Text},
                        new SelectListItem {Value = Convert.ToString(ReferenceOperator.NotMatchesAny), Text = T("Not mathes any").Text}
                    };
                    return Shape.FilterEditors_ReferenceFilter(Id: FormName, Operators: operators);
                };

            context.Form(FormName, form);
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            var op = (ReferenceOperator)Enum.Parse(typeof(ReferenceOperator), (string)formState.Operator);
            string value = formState.Value;
            var items = !string.IsNullOrWhiteSpace(value)
                ? value.Split('&').Select(int.Parse).ToArray()
                : new int[] {};
            fieldName = fieldName.Split('.')[1];
            switch (op) {
                case ReferenceOperator.MatchesAny:
                    return T("{0} matches any in the {1} selected items", fieldName, items.Length);
                case ReferenceOperator.NotMatchesAny:
                    return T("{0} not matches any in the {1} selected items", fieldName, items.Length);
            }
            return new LocalizedString(fieldName);
        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property) {
            string value = formState.Value;
            if (string.IsNullOrWhiteSpace(value)) {
                return null;
            }

            var op = (ReferenceOperator)Enum.Parse(typeof(ReferenceOperator), (string)formState.Operator);
            var items = value.Split('&').Select(int.Parse).ToArray();
            switch (op) {
                case ReferenceOperator.MatchesAny:
                    return x => x.In(property, items);
                case ReferenceOperator.NotMatchesAny:
                    return x => x.Or(l => l.Not(a => a.In(property, items)), r => r.IsNull(property));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum ReferenceOperator {
        MatchesAny,
        NotMatchesAny
    }
}