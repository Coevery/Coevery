using System.Web.Mvc;
using Orchard;
using Orchard.Localization;

namespace Coevery.OptionSet.Controllers {
    public class SystemAdminController : Controller {
        public SystemAdminController(IOrchardServices orchardServices) {
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }

        public ActionResult List(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}