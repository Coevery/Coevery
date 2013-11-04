using System.Web.Mvc;
using Coevery.Themes;

namespace Coevery.Core.Common.Controllers {
    [Themed]
    public class ErrorController : Controller {

        public ActionResult NotFound(string url) {
            return HttpNotFound();
        }
    }
}