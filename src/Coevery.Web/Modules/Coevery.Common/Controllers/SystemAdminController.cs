using System.Web.Mvc;
using Coevery.Common.Admin;
using Coevery.Themes;

namespace Coevery.Common.Controllers {
    public class SystemAdminController : Controller {
        [SystemAdmin, Themed]
        public ActionResult Index(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}