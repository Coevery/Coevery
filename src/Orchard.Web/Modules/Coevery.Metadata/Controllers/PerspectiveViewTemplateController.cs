using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Coevery.Metadata.Controllers
{
    public class PerspectiveViewTemplateController:Controller
    {
        public ActionResult List(string id)
        {
            return View();
        }

        public ActionResult EditPerspective(string id)
        {
            return View();
        }

        public ActionResult EditNavigationItem(string id)
        {
            return View();
        }
    }
}