using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Coevery.Metadata.Controllers
{
    public class SystemAdminController : Controller
    {
        public ActionResult EditOneToMany(string id)
        {
            return View();
        }

        public ActionResult EditManyToMany(string id)
        {
            return View();
        }
    }
}