using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.CRM.Convertor.Models;

namespace Coevery.CRM.Convertor.Controllers
{
    public class HomeController : Controller {
        public ActionResult Index() {
            return View(QuotesProvider.GetQuotes());
        }

        public ActionResult IndexPartial()
        {
            return PartialView(QuotesProvider.GetQuotes());
        }
    }
}