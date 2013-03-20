using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Themes;

namespace Coevery.Leads.Controllers
{

    public class HomeController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly dynamic _shapeFactory;

        public HomeController(IContentManager contentManager,
            IShapeFactory shapeFactory) {
            _contentManager = contentManager;
            _shapeFactory = shapeFactory;
        }

        [Themed]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult Detail(string returnUrl)
        {
            var id = "Lead";
            var contentItem = _contentManager.New(id);

            dynamic model = _contentManager.BuildEditor(contentItem);

            return View((object)model);
        }

        public ActionResult List(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}