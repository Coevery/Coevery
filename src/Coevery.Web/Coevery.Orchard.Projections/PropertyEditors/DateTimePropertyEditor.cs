using System;
using System.Linq;
using Coevery.Orchard.Projections.ModelBinding;
using Coevery.Orchard.Projections.PropertyEditors.Forms;

namespace Coevery.Orchard.Projections.PropertyEditors {
    public class DateTimePropertyEditor : IPropertyEditor {
        private readonly IWorkContextAccessor _workContextAccessor;

        public DateTimePropertyEditor(
            IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;
        }

        public bool CanHandle(Type type) {
            return new[] { typeof(DateTime), typeof(DateTime?) }.Contains(type);
        }

        public string FormName {
            get { return DateTimePropertyForm.FormName; }
        }

        public dynamic Format(dynamic display, object value, dynamic formState) {
            var culture = _workContextAccessor.GetContext().CurrentCulture;
            return DateTimePropertyForm.FormatDateTime(display, Convert.ToDateTime(value, new System.Globalization.CultureInfo(culture)), formState, culture);
        }
    }
}