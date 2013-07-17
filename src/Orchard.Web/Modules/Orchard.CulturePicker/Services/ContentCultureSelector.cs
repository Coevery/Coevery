using System;
using System.Web;
using System.Web.Routing;
using Orchard.Alias;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization.Services;

namespace Orchard.CulturePicker.Services {
    public class ContentCultureSelector : ICultureSelector {
        public const int SelectorPriority = -4; //priority is higher than SiteCultureSelector priority (-5), But lower than CookieCultureSelector(-3)
        private readonly IAliasService _aliasService;
        private readonly IOrchardServices _orchardServices;

        private CultureSelectorResult _evaluatedResult;
        private bool _isEvaluated;

        public ContentCultureSelector(IAliasService aliasService, IOrchardServices orchardServices) {
            _aliasService = aliasService;
            _orchardServices = orchardServices;
        }

        #region ICultureSelector Members

        public CultureSelectorResult GetCulture(HttpContextBase context) {
            if (!_isEvaluated) {
                _isEvaluated = true;
                _evaluatedResult = EvaluateResult(context);
            }

            return _evaluatedResult;
        }

        #endregion

        #region Helpers

        private CultureSelectorResult EvaluateResult(HttpContextBase context) {
            if (context == null || context.Request == null) {
                return null;
            }

            //TODO: check for a more efficient way to get content item for the current request
            string relativePath = Utils.GetAppRelativePath(context.Request.Url.AbsolutePath, context.Request);
            relativePath = HttpUtility.UrlDecode(relativePath);
            RouteValueDictionary routeValueDictionary = _aliasService.Get(relativePath);

            if (routeValueDictionary == null) {
                return null;
            }

            int routeId = Convert.ToInt32(routeValueDictionary["Id"]);

            ContentItem content = _orchardServices.ContentManager.Get(routeId, VersionOptions.Published);
            if (content == null) {
                return null;
            }

            //NOTE: we can't use ILocalizationService.GetContentCulture for this, because it causes circular dependency
            var localized = content.As<ILocalizableAspect>();
            if (localized == null || string.IsNullOrEmpty(localized.Culture)) {
                return null;
            }

            return new CultureSelectorResult {Priority = SelectorPriority, CultureName = localized.Culture};
        }

        #endregion
    }
}