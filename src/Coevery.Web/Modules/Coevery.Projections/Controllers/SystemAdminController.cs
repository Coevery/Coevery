using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Coevery.Common.Extensions;
using Coevery.Entities.Services;
using Coevery.Forms.Services;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Coevery.Mvc;
using Coevery.Localization;
using Coevery.Security;

namespace Coevery.Projections.Controllers {
    public class SystemAdminController : Controller {
        private readonly IProjectionService _projectionService;
        private readonly IContentMetadataService _contentMetadataService;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;
        private readonly IProjectionManager _projectionManager;
        private readonly IFormManager _formManager;

        public SystemAdminController(
            ICoeveryServices services,
            IContentMetadataService contentMetadataService,
            IContentDefinitionExtension contentDefinitionExtension,
            IFormManager formManager,
            IProjectionManager projectionManager,
            IProjectionService projectionService) {
            _contentDefinitionExtension = contentDefinitionExtension;
            _contentMetadataService = contentMetadataService;
            _projectionService = projectionService;
            _projectionManager = projectionManager;
            _formManager = formManager;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public ICoeveryServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult List(string id) {
            var model = new EntityViewListModel {
                Layouts = _projectionManager.DescribeLayouts()
                    .SelectMany(type => type.Descriptors)
                    .Where(type => type.Category == "Grids")
            };
            return View(model);
        }

        public ActionResult Create(string id, string category, string type) {
            var entityName = _contentDefinitionExtension.GetEntityNameFromCollectionName(id);
            if (entityName != null) {
                id = entityName;
            }
            if (!_contentMetadataService.CheckEntityPublished(id)) {
                return Content(T("The \"{0}\" hasn't been published!", id).Text);
            }

            var viewModel = new ProjectionEditViewModel {
                ItemContentType = id.ToPartName(),
                DisplayName = string.Empty,
                Fields = _projectionService.GetFieldDescriptors(id, -1),
                Layout = _projectionManager.DescribeLayouts()
                    .SelectMany(descr => descr.Descriptors)
                    .FirstOrDefault(descr => descr.Category == category && descr.Type == type),
            };
            if (viewModel.Layout == null) {
                return HttpNotFound();
            }
            viewModel.Form = _formManager.Build(viewModel.Layout.Form) ?? Services.New.EmptyForm();
            viewModel.Form.Fields = viewModel.Fields;
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
        public ActionResult EditPost(int id, ProjectionEditViewModel viewModel, string returnUrl) {
            viewModel.Layout = _projectionManager.DescribeLayouts()
                    .SelectMany(descr => descr.Descriptors)
                    .FirstOrDefault(descr => descr.Category == viewModel.Layout.Category && descr.Type == viewModel.Layout.Type);
            if (viewModel.Layout == null) {
                return HttpNotFound();
            }
            _formManager.Validate(new ValidatingContext { FormName = viewModel.Layout.Form, ModelState = ModelState, ValueProvider = ValueProvider });
            if (!ModelState.IsValid) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var viewid = _projectionService.EditPost(id, viewModel, viewModel.PickedFields);
            return Json(new { id = viewid });
        }
    }
}