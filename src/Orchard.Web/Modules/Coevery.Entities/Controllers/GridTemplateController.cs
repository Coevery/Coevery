using System.Web.Mvc;

namespace Coevery.Entities.Controllers {

    public class GridTemplateController : Controller {

        public ActionResult DefaultFooterTemplate() {
            return View();
        }
    }
}