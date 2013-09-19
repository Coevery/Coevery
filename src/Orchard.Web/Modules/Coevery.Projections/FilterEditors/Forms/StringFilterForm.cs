using System;
using System.Collections.Generic;
using System.Web.Mvc;
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