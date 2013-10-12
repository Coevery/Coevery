using System.Web.Mvc;
using Coevery.Core.FrontMenu;
using Orchard.Themes;

namespace Coevery.Core.Controllers {
    public class HomeController : Controller {
        [FrontMenu, Themed]
        public ActionResult Index(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}