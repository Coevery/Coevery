using System;
using System.Web;
using System.Web.Mvc;
using Orchard.Autoroute.Models;
using Orchard.CulturePicker.Services;
using Orchard.Localization;
using Orchard.Mvc.Extensions;

namespace Orchard.CulturePicker.Controllers {
    public class UserCultureController : Controller {
        private readonly ILocalizableContentService _localizableContentService;

        public UserCultureController(IOrchardServices services, ILocalizableContentService localizableContentService) {
            Services = services;
            _localizableContentService = localizableContentService;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult ChangeCulture(string cultureName) {
            if (string.IsNullOrEmpty(cultureName)) {
                throw new ArgumentNullException(cultureName);
            }

            string returnUrl = Utils.GetReturnUrl(Services.WorkContext.HttpContext.Request);

            AutoroutePart currentRoutePart;
            //returnUrl may not correspond to any content and we use "Try" approach
            if (_localizableContentService.TryGetRouteForUrl(returnUrl, out currentRoutePart)) {
                AutoroutePart localizedRoutePart;
                //content may not have localized version and we use "Try" approach
                if (_localizableContentService.TryFindLocalizedRoute(currentRoutePart.ContentItem, cultureName, out localizedRoutePart)) {
                    returnUrl = localizedRoutePart.Path;
                }
            }

            SaveCultureCookie(cultureName);

            //support for Orchard < 1.6
            //TODO: discontinue in 2013 Q2
            Version orchardVersion = Utils.GetOrchardVersion();
            if (orchardVersion < new Version(1, 6)) {
                returnUrl = Url.Encode(returnUrl);
            }
            else {
                if (!returnUrl.StartsWith("~/")) {
                    returnUrl = "~/" + returnUrl;
                }
            }

            return this.RedirectLocal(returnUrl);
        }

        #region Helpers

        //Saves culture information to cookie
        private void SaveCultureCookie(string cultureName) {
            HttpContextBase httpContext = Services.WorkContext.HttpContext;
            HttpRequestBase request = httpContext.Request;

            var cultureCookie = new HttpCookie(CookieCultureSelector.CultureCookieName);
            cultureCookie.Values.Add(CookieCultureSelector.CurrentCultureFieldName, cultureName);
            cultureCookie.Expires = DateTime.Now.AddYears(1);

            //setting up domain for cookie allows to share it to sub-domains as well
            //if non-default port is used, we consider it as a testing environment without sub-domains
            if (request.Url != null && request.Url.IsDefaultPort) {
                // '.' prefix means, that cookie will be shared to sub-domains
                cultureCookie.Domain = "." + request.Url.Host;
            }

            httpContext.Response.Cookies.Add(cultureCookie);
        }

        #endregion
    }
}