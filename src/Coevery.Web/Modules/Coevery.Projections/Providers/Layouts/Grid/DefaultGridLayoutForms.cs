using System;
using System.Collections.Generic;
using Coevery.DisplayManagement;
using Coevery.Forms.Services;
using Coevery.Localization;

namespace Coevery.Projections.Providers.Layouts.Grid {

    public class DefaultGridLayoutForms : IFormProvider {
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public DefaultGridLayoutForms(
            IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {
                    var f = Shape.DefaultGrid_Edit(State: new Dictionary<string,string> {
                        {"PageRowCount", "50"},
                        {"SortedBy", string.Empty},
                        {"SortMode", string.Empty}
                    }, Fields: null);
                    return f;
                };

            context.Form("DefaultGridLayout", form);
        }
    }

    public class DefaultGridLayoutFormsValitator : FormHandler {
        public Localizer T { get; set; }

        public override void Validating(ValidatingContext context) {
            if (context.FormName == "DefaultGridLayout") {
                int value;
                if (!Int32.TryParse(context.ValueProvider.GetValue("State[PageRowCount]").AttemptedValue, out value)) {
                    context.ModelState.AddModelError("PageRowCount", T("The page row count must be a valid number.").Text);
                }
            }
        }
    }

}