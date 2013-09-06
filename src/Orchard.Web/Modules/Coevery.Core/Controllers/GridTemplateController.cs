using System.Web.Mvc;

namespace Coevery.Core.Controllers {

    public class GridTemplateController : Controller {

        public ActionResult DefaultFooterTemplate() {
            return View();
        }
    }
}