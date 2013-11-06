using System.Web.Mvc;
using Coevery.Themes;

namespace Coevery.Common.Controllers {
    public class HomeController : Controller {
        [Themed]
        public ActionResult Index(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}