using System;
using Coevery.ContentManagement;
using Coevery.DisplayManagement;
using Coevery.Forms.Services;
using Coevery.Localization;

namespace Coevery.Projections.FilterEditors.Forms {
    public class OrchardBooleanFilterForm : IFormProvider {
        public const string FormName = "BooleanFilter";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public OrchardBooleanFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {

                    var f = Shape.Form(
                        _Options: Shape.Fieldset(
                            _ValueUndefined: Shape.Radio(
                                Id: "value-undefined", Name: "Value",
                                Title: T("Undefined"), Value: "undefined"
                                ),
                            _ValueTrue: Shape.Radio(
                                Id: "value-true", Name: "Value",
                                Title: T("Yes"), Value: "true", Checked: true
                                ),
                            _ValueFalse: Shape.Radio(
                                Id: "value-false", Name: "Value",
                                Title: T("No"), Value: "false"
                                ),
                            Description: T("Enter the value the string should be.")
                        ));

                    return f;
                };

            context.Form(FormName, form);

        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            
            if(formState.Value == "undefined") {
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
                return x => x.Gt(property, (long)0);
            }

            return x => x.Eq(property, (long)0);
        }
    }
}