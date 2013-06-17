using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.AdminNavigation;

namespace Coevery.Core.Controllers
{
    public class SysAdminController : Controller
    {
        [SysAdmin]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}