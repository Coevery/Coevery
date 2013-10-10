using System;
using Orchard.ContentManagement;
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
                shape => Shape.FilterEditors_BooleanFilter(Id: FormName);

            context.Form(FormName, form);
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            bool value = Convert.ToBoolean(formState.Value);
            fieldName = fieldName.Split('.')[1];
            return value
                ? T("{0} is true", fieldName)
                : T("{0} is false", fieldName);
        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property) {
            bool value = Convert.ToBoolean(formState.Value);

            if (value) {
                return x => x.Gt(property, (long) 0);
            }

            return x => x.Or(l => l.Eq(property, (long) 0), r => r.IsNull(property));
        }
    }
}