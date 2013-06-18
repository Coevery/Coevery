using System.Web.Mvc;
using Coevery.Entities.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;

namespace Coevery.Entities.Controllers
{
    public class SystemAdminController : Controller
    {
        private readonly IContentDefinitionService _contentDefinitionService;

        public SystemAdminController(IOrchardServices orchardServices, 
            IContentDefinitionService contentDefinitionService)
        {
            Services = orchardServices;
            _contentDefinitionService = contentDefinitionService;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult List(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult Create() {
            //if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to create a content type.")))
            //    return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(string.Empty);

            return View(typeViewModel);
        }
    }
}