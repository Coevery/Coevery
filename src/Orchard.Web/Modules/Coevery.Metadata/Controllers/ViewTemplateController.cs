using System.Collections.Generic;
using System.Web.Mvc;
using Coevery.Metadata.Services;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Metadata.Controllers
{
    public class ViewTemplateController : Controller
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        public ViewTemplateController(IContentDefinitionService contentDefinitionService)
        {
            _contentDefinitionService = contentDefinitionService;
        }
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Detail()
        {
            var model = _contentDefinitionService.GetTempEditTypeViewModel();
            return View(model);
        }
    }
}
