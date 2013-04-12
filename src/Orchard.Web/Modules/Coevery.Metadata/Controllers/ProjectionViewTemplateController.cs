using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Web.Mvc;
using Coevery.Metadata.Services;
using Coevery.Metadata.ViewModels;
using Orchard;
using Orchard.Core.Contents.Controllers;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Security;


namespace Coevery.Metadata.Controllers
{
    public class ProjectionViewTemplateController:Controller
    {
        private readonly IProjectionService _projectionService;

        public ProjectionViewTemplateController(
             IOrchardServices services,
            IShapeFactory shapeFactory,
            IProjectionService projectionService)
        {
            _projectionService = projectionService;
            Services = services;

            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult List(string id)
        {
            return View();
        }

        public ActionResult Create(string id)
        {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id))
            {
                id = pluralService.Singularize(id);
            }

           var viewModel = _projectionService.CreateTempProjection(id);
           return RedirectToAction("Edit", new { subId = viewModel.Id });
        }

        public ActionResult Edit(int subId)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to edit queries")))
                return new HttpUnauthorizedResult();
            ProjectionEditViewModel viewModel = _projectionService.GetProjectionViewModel(subId);

            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int subId, ProjectionEditViewModel viewModel, IEnumerable<string> picklist, string returnUrl)
        {
            bool suc = _projectionService.EditPost(subId, viewModel, picklist);
            return new EmptyResult();
        }
       
    }
}