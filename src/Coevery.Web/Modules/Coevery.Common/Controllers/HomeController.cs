using System.Web.Mvc;
using Coevery.Common.FrontMenu;
using Coevery.Themes;

namespace Coevery.Common.Controllers {
    public class HomeController : Controller {
        [FrontMenu, Themed]
        public ActionResult Index(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}