using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Projections.ModelBinding;
using Orchard.Projections.PropertyEditors.Forms;

namespace Coevery.Projections.Providers.PropertyEditors {
    public class CoeveryNumericPropertyEditor : IPropertyEditor {
        private readonly IWorkContextAccessor _workContextAccessor;

        public CoeveryNumericPropertyEditor(IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;
        }

        public bool CanHandle(Type type) {
            return new[] {
                typeof(double?), 
                typeof(decimal?)
            }.Contains(type);
        }

        public string FormName {
            get { return NumericPropertyForm.FormName; }
        }

        public dynamic Format(dynamic display, object value, dynamic formState) {
            var culture = CultureInfo.CreateSpecificCulture(_workContextAccessor.GetContext().CurrentCulture);
            var number = Convert.ToDecimal(value, culture);
            string formatOption = formState.Format;

            /*Currently not need to define the place for currency symbol
            string prefix = formState.Prefix;
            if (!String.IsNullOrEmpty(prefix)) {
                result = prefix + result;
            }

            string suffix = formState.Suffix;
            if (!String.IsNullOrEmpty(suffix)) {
                result = result + suffix;
            }
             */

            return number.ToString(formatOption, culture);
        }
    }
}