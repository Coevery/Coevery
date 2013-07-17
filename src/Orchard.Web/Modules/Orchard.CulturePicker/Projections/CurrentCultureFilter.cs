/* 
 * Disclaimer
 * This filter has been taken form pull request Localization
 * Changeset #6842afdfd4c9
 * Author: Geert Doornbos
 */
//TODO: discontinue in 2013 Q2 (this filter already merged into Orchard 1.6)


using System;
using Orchard.ContentManagement;
using Orchard.CulturePicker.Services;
using Orchard.Events;
using Orchard.Localization;
using Orchard.Localization.Models;
using Orchard.Localization.Services;

namespace Orchard.CulturePicker.Projections {
    public interface IFilterProvider : IEventHandler {
        void Describe(dynamic describe);
    }

    public class CurrentCultureFilter : IFilterProvider {
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public CurrentCultureFilter(IWorkContextAccessor workContextAccessor, ICultureManager cultureManager) {
            _workContextAccessor = workContextAccessor;
            _cultureManager = cultureManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        #region IFilterProvider Members

        public void Describe(dynamic describe) { 
            Version orchardVersion = Utils.GetOrchardVersion();
            if (orchardVersion < new Version(1, 6)) {
                describe.For("Localization", T("Localization"), T("Localization"))
                    .Element("ForCurrentCulture", T("For current culture"), T("Localized content items for current culture"),
                             (Action<dynamic>)ApplyFilter,
                             (Func<dynamic, LocalizedString>)DisplayFilter,
                             null
                    );
            }
        }

        #endregion

        public void ApplyFilter(dynamic context) {
            string currentCulture = _workContextAccessor.GetContext().CurrentCulture;
            int currentCultureId = _cultureManager.GetCultureByName(currentCulture).Id;

            var query = (IHqlQuery) context.Query;
            context.Query = query.Where(x => x.ContentPartRecord<LocalizationPartRecord>(), x => x.Eq("CultureId", currentCultureId));
        }

        public LocalizedString DisplayFilter(dynamic context) {
            return T("For current culture");
        }
    }
}