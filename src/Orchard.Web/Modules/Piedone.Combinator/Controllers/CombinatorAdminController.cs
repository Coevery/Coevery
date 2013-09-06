using System.Web.Mvc;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.UI.Admin;
using Piedone.Combinator.Services;

namespace Piedone.Combinator.Controllers
{
    [Admin, OrchardFeature("Piedone.Combinator")]
    public class CombinatorAdminController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICacheFileService _cacheFileService;

        public CombinatorAdminController(
            IOrchardServices orchardServices,
            ICacheFileService cacheFileService)
        {
            _orchardServices = orchardServices;
            _cacheFileService = cacheFileService;
        }

        [HttpPost]
        //public ActionResult EmptyCache(string returnUrl = "") Without AJAX
        public void EmptyCache()
        {
            if (_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner))
            {
                _cacheFileService.Empty();
            }

            // Since we are calling this via AJAX, this is not necessary. But the AJAX implementation is not the best.
            //return this.RedirectLocal(returnUrl); // this necessary, as this is from an extension (Orchard.Mvc.Extensions.ControllerExtensions)
        }
    }
}
