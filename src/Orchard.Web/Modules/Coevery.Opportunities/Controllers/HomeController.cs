using System.Web.Mvc;
using Orchard.Themes;

namespace Coevery.Opportunities.Controllers
{
    
    public class HomeController : Controller
    {
        [Themed]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult List(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult Detail(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}