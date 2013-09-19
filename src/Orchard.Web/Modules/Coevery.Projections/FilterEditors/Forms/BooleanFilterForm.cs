using System;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace Coevery.Projections.FilterEditors.Forms {
    [OrchardSuppressDependency("Orchard.Projections.FilterEditors.Forms.BooleanFilterForm")]
    public class BooleanFilterForm : IFormProvider {
        public const string FormName = "BooleanFilter";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public BooleanFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => Shape.FilterEditors_BooleanFilter(Id:FormName);

            context.Form(FormName, form);
        }
    }
}