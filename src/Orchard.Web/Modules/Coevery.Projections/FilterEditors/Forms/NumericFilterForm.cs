using System;
using System.Collections.Generic;
using System.Web.Mvc;
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