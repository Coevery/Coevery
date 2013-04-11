using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.Metadata.Services;
using Coevery.Metadata.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents.Controllers;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Descriptors.Layout;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Projections.ViewModels;
using Orchard.Security;
using Orchard.Settings;

namespace Coevery.Metadata.Controllers
{
    public class ProjectionViewTemplateController:Controller
    {
        private readonly IOrchardServices _services;
        private readonly ISiteService _siteService;
        private readonly IQueryService _queryService;
        private readonly IProjectionManager _projectionManager;
        private readonly IContentManager _contentManager;
        private readonly IProjectionService _projectionService;

        public ProjectionViewTemplateController(
             IOrchardServices services,
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IQueryService queryService,
            IProjectionManager projectionManager,
            IContentManager contentManager,
            IProjectionService projectionService)
        {
            _services = services;
            _siteService = siteService;
            _queryService = queryService;
            _projectionManager = projectionManager;
            _contentManager = contentManager;
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