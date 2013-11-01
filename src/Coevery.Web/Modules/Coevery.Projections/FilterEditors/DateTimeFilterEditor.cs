using System;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Projections.FilterEditors.Forms;
using Coevery.Services;

namespace Coevery.Projections.FilterEditors {
    public class DateTimeFilterEditor : IFilterEditor {
        private readonly IClock _clock;

        public DateTimeFilterEditor(IClock clock) {
            _clock = clock;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public bool CanHandle(Type type) {
            return new[] {
                typeof(DateTime),
                typeof(DateTime?),
            }.Contains(type);
        }

        public string FormName {
            get { return DateTimeFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> Filter(string property, dynamic formState) {
            return DateTimeFilterForm.GetFilterPredicate(formState, property, _clock.UtcNow);
        }

        public LocalizedString Display(string property, dynamic formState) {
            return DateTimeFilterForm.DisplayFilter(property, formState, T);
        }
    }
}