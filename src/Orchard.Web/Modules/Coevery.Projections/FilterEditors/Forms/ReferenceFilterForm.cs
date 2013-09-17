using System;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;

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
                shape => Shape.FilterEditors_ReferenceFilter(Id: FormName);

            context.Form(FormName, form);
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            if (formState.Value == "undefined") {
                return T("{0} is undefined", fieldName);
            }

            bool value = Convert.ToBoolean(formState.Value);

            return value
                ? T("{0} is true", fieldName)
                : T("{0} is false", fieldName);
        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property) {
            if (formState.Value == "undefined") {
                return x => x.IsNull(property);
            }

            bool value = Convert.ToBoolean(formState.Value);

            if (value) {
                return x => x.Gt(property, (long) 0);
            }

            return x => x.Eq(property, (long) 0);
        }
    }
}