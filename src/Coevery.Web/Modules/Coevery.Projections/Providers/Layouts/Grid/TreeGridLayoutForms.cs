using System;
using System.Collections.Generic;
using Coevery.DisplayManagement;
using Coevery.Forms.Services;
using Coevery.Localization;

namespace Coevery.Projections.Providers.Layouts.Grid {

    public class TreeGridLayoutForms : IFormProvider {
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public TreeGridLayoutForms(
            IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {
                    var f = Shape.TreeGrid_Edit(State: new Dictionary<string, string> {
                        {"ExpandField", string.Empty},
                        {"ParentField", string.Empty}
                    }, Fields: null);
                    return f;
                };
            context.Form("TreeGridLayout", form);
        }
    }

    public class TreeGridLayoutFormsValitator : FormHandler {
        public Localizer T { get; set; }

        public override void Validating(ValidatingContext context) {
            if (context.FormName == "TreeGridLayout") {
                if (string.IsNullOrWhiteSpace(context.ValueProvider.GetValue("State[ParentField]").AttemptedValue)) {
                    context.ModelState.AddModelError("ParentField", T("The parent field is required.").Text);
                }
                if (string.IsNullOrWhiteSpace(context.ValueProvider.GetValue("State[ExpandField]").AttemptedValue)) {
                    context.ModelState.AddModelError("ExpandField", T("The field to expand tree is required.").Text);
                }
            }
        }
    }
}