using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.Core.Admin;
using Orchard.Themes;

namespace Coevery.Core.Controllers
{
    public class SystemAdminController : Controller
    {
        [SystemAdmin, Themed]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}