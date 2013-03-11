using System.Web.Mvc;
using Orchard.Themes;

namespace Coevery.Leads.Controllers
{
    public class DetailController : Controller
    {
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Detail");
        }
    }
}