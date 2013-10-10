using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.Entities.Services;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Orchard;
using Orchard.Mvc;
using Orchard.Localization;
using Orchard.Security;

namespace Coevery.Projections.Controllers {
    public class SystemAdminController : Controller {
        private readonly IProjectionService _projectionService;
        private readonly IContentMetadataService _contentMetadataService;

        public SystemAdminController(
            IOrchardServices services,
            IContentMetadataService contentMetadataService,
            IProjectionService projectionService) {
            _contentMetadataService = contentMetadataService;
            _projectionService = projectionService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult List(string id) {
            return View(new Dictionary<string, string> { { "PublishTip", T("Works when the entity has been published.").Text } });
        }

        public ActionResult Create(string id) {   
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id)) {
                id = pluralService.Singularize(id);
            }
            if (!_contentMetadataService.CheckEntityPublished(id)) {
                return Content(T("The \"{0}\" hasn't been published!", id).Text);
            }

            var viewModel = new ProjectionEditViewModel {
                ItemContentType = id,
                DisplayName = string.Empty,
                Fields = _projectionService.GetFieldDescriptors(id)
            };
            return View("Edit", viewModel);
        }

        public ActionResult Edit(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to edit queries"))) {
                return new HttpUnauthorizedResult();
            }
            ProjectionEditViewModel viewModel = _projectionService.GetProjectionViewModel(id);

            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int id, ProjectionEditViewModel viewModel, string picklist, string returnUrl) {
            var pickArray = picklist.Split(new[] {'$'}, StringSplitOptions.RemoveEmptyEntries);
            _projectionService.EditPost(id, viewModel, pickArray);
            return Json(new { id = id});
        }
    }
}