using System.Web.Mvc;
using Coevery;
using Coevery.Localization;

namespace Coevery.OptionSet.Controllers {
    public class SystemAdminController : Controller {
        public SystemAdminController(ICoeveryServices coeveryServices) {
            Services = coeveryServices;
            T = NullLocalizer.Instance;
        }

        public ICoeveryServices Services { get; private set; }
        public Localizer T { get; set; }

        public ActionResult List(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}