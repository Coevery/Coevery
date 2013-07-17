using System.Collections.Generic;
using System.Linq;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization.Models;
using Orchard.Localization.Services;

namespace Orchard.CulturePicker.Services {
    public class LocalizableContentService : ILocalizableContentService {
        private readonly IContentManager _contentManager;
        private readonly ICultureManager _cultureManager;
        private readonly ILocalizationService _localizationService;

        public LocalizableContentService(ILocalizationService localizationService, ICultureManager cultureManager, IContentManager contentManager) {
            _localizationService = localizationService;
            _cultureManager = cultureManager;
            _contentManager = contentManager;
        }

        //Finds route part for the specified URL
        //Returns true if specified url corresponds to some content and route exists; otherwise - false

        #region ILocalizableContentService Members

        public bool TryGetRouteForUrl(string url, out AutoroutePart route) {
            //first check for route (fast, case sensitive, not precise)
            route = _contentManager.Query<AutoroutePart, AutoroutePartRecord>()
                .ForVersion(VersionOptions.Published)
                .Where(r => r.DisplayAlias == url)
                .List()
                .FirstOrDefault();

            return route != null;
        }

        //Finds localized route part for the specified content and culture
        //Returns true if localized url for content and culture exists; otherwise - false
        public bool TryFindLocalizedRoute(ContentItem routableContent, string cultureName, out AutoroutePart localizedRoute) {
            if (!routableContent.Parts.Any(p => p.Is<ILocalizableAspect>())) {
                localizedRoute = null;
                return false;
            }

            //var siteCulture = _cultureManager.GetCultureByName(_cultureManager.GetSiteCulture());
            IEnumerable<LocalizationPart> localizations = _localizationService.GetLocalizations(routableContent, VersionOptions.Published);

            ILocalizableAspect localizationPart = null, siteCultureLocalizationPart = null;
            foreach (LocalizationPart l in localizations) {
                if (l.Culture.Culture == cultureName) {
                    localizationPart = l;
                    break;
                }
                if (l.Culture == null && siteCultureLocalizationPart == null) {
                    siteCultureLocalizationPart = l;
                }
            }

            //try get localization part for default site culture
            if (localizationPart == null) {
                localizationPart = siteCultureLocalizationPart;
            }

            if (localizationPart == null) {
                localizedRoute = null;
                return false;
            }

            ContentItem localizedContentItem = localizationPart.ContentItem;
            localizedRoute = localizedContentItem.Parts.Single(p => p is AutoroutePart).As<AutoroutePart>();
            return true;
        }

        #endregion
    }
}