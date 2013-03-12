using System.Web.Mvc;
using Orchard.Themes;

namespace Coevery.Opportunities.Controllers
{
    [Themed]
    public class HomeController : Controller
    {
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}