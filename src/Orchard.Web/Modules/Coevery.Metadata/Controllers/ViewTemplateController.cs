using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.ContentManagement;

namespace Coevery.Leads.Controllers
{
    public class ViewTemplateController : Controller
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Detail()
        {
            return View();
        }
    }
}
