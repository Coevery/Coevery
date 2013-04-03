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
        public ActionResult List(string id)
        {
            return View();
        }

        public ActionResult Edit(string id) {
            var model = _contentDefinitionService.GetTempEditTypeViewModel();
            return View(model);
        }

        public ActionResult Create(string id, int? containerId) {
            return Edit(id);
        }
    }
}
