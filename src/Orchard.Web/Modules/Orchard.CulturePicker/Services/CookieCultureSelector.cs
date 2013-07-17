using System;
using System.Web;
using Orchard.Localization.Services;

namespace Orchard.CulturePicker.Services {
    public class CookieCultureSelector : ICultureSelector {
        public const string CultureCookieName = "cultureData";
        public const string CurrentCultureFieldName = "currentCulture";
        public const int SelectorPriority = -3; //priority is higher than SiteCultureSelector priority (-5)

        private CultureSelectorResult _evaluatedResult;
        private bool _isEvaluated;

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
            if (context == null || context.Request == null || context.Request.Cookies == null) {
                return null;
            }

            HttpCookie cultureCookie = context.Request.Cookies[context.Request.AnonymousID + CultureCookieName];

            if (cultureCookie == null) {
                return null;
            }

            string currentCultureName = cultureCookie[CurrentCultureFieldName];
            return String.IsNullOrEmpty(currentCultureName) ? null : new CultureSelectorResult {Priority = SelectorPriority, CultureName = currentCultureName};
        }

        #endregion
    }
}