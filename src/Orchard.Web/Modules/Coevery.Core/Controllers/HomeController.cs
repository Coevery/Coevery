using System.Collections.Generic;
using System.Web.Mvc;
using Coevery.Core.FrontMenu;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Themes;

namespace Coevery.Leads.Controllers
{
    public class HomeController : Controller
    {

        [FrontMenuAttribute, Themed]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}