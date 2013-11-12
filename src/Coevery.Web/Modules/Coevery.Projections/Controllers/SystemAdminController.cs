using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.Common.Extensions;
using Coevery.Entities.Services;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Coevery;
using Coevery.Mvc;
using Coevery.Localization;
using Coevery.Security;

namespace Coevery.Projections.Controllers {
    public class SystemAdminController : Controller {
        private readonly IProjectionService _projectionService;
        private readonly IContentMetadataService _contentMetadataService;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;

        public SystemAdminController(
            ICoeveryServices services,
            IContentMetadataService contentMetadataService,
            IContentDefinitionExtension contentDefinitionExtension,
            IProjectionService projectionService) {
            _contentDefinitionExtension = contentDefinitionExtension;
            _contentMetadataService = contentMetadataService;
            _projectionService = projectionService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public ICoeveryServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult List(string id) {
            return View(new Dictionary<string, string> { { "PublishTip", T("Works when the entity has been published.").Text } });
        }

        public ActionResult Create(string id) {   
            id = _contentDefinitionExtension.GetEntityNameFromCollectionName(id);
            if (!_contentMetadataService.CheckEntityPublished(id)) {
                return Content(T("The \"{0}\" hasn't been published!", id).Text);
            }

            var viewModel = new ProjectionEditViewModel {
                ItemContentType = id.ToPartName(),
                DisplayName = string.Empty,
                Fields = _projectionService.GetFieldDescriptors(id,-1)
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
        public ActionResult EditPOST(int id, ProjectionEditViewModel viewModel, string returnUrl) {
            viewModel.ItemContentType = viewModel.ItemContentType;
            var viewid = _projectionService.EditPost(id, viewModel, viewModel.PickedFields);
            return Json(new { id = viewid });
        }
    }
}