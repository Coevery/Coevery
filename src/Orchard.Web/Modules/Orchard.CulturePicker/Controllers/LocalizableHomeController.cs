using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Alias;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.CulturePicker.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Themes;

namespace Orchard.CulturePicker.Controllers {
    [HandleError]
    [OrchardFeature("Orchard.CulturePicker.HomePageRedirect")]
    public class LocalizableHomeController : Controller {
        private readonly IAliasService _aliasService;
        private readonly ICultureManager _cultureManager;
        private readonly ILocalizableContentService _localizableContentService;
        private readonly IOrchardServices _orchardServices;

        public LocalizableHomeController(IOrchardServices orchardServices, ICultureManager cultureManager, ILocalizableContentService localizableContentService, IAliasService aliasService) {
            _orchardServices = orchardServices;
            _cultureManager = cultureManager;
            _localizableContentService = localizableContentService;
            _aliasService = aliasService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        //it's igly, but works
        //TODO: find more elegant way without controller
        [Themed]
        public ActionResult Index() {
            HttpContextBase httpContext = _orchardServices.WorkContext.HttpContext;

            RouteValueDictionary homepageRouteValueDictionary = _aliasService.Get(string.Empty);
            int routeId = Convert.ToInt32(homepageRouteValueDictionary["Id"]);

            ContentItem content = _orchardServices.ContentManager.Get(routeId, VersionOptions.Published);
            if (content == null) {
                return new HttpNotFoundResult();
            }

            string currentCultureName = _cultureManager.GetCurrentCulture(httpContext);
            AutoroutePart localizedRoutePart;
            //content may not have localized version and we use "Try" approach
            if (_localizableContentService.TryFindLocalizedRoute(content, currentCultureName, out localizedRoutePart)) {
                string returnUrl = localizedRoutePart.Path;

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

                return this.RedirectLocal(returnUrl, (Func<ActionResult>) null);
            }

            dynamic model = _orchardServices.ContentManager.BuildDisplay(content);
            return new ShapeResult(this, model);
        }
    }
}