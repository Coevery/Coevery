

using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using Coevery.Core.Services;
using Coevery.Core.ViewModels;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Contents;
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Contents.Settings;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Html;
using Orchard.Projections.Models;
using Orchard.Settings;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;

namespace Coevery.Core.Controllers
{
    public class ContentViewTemplateController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;
        private readonly IViewPartService _projectionService;

        public ContentViewTemplateController(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            ITransactionManager transactionManager,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IViewPartService projectionService) {
            Services = orchardServices;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            _transactionManager = transactionManager;
            _siteService = siteService;
            _projectionService = projectionService;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult List(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return  new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string moduleName = id;
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            id = pluralService.Singularize(id);
            var contentType = _contentDefinitionManager.GetTypeDefinition(id);
            int viewId = _projectionService.GetProjectionId(id);

            dynamic viewModel = Services.New.ViewModel();
            viewModel.DisplayName(contentType.DisplayName);
            viewModel.TypeDefinition(contentType);
           // viewModel.Columns(this.GetViewColumns(viewId));
            viewModel.ModuleName(moduleName);
            viewModel.ViewId(viewId);
            //var model = GetListModel(viewId);
            //viewModel.ListViewModel(model);
            return View(viewModel);
        }

        private IEnumerable<PropertyRecord> GetViewColumns(int viewId)
        {
            List<PropertyRecord> re = new List<PropertyRecord>();
            if (viewId == -1) return re;
            var projectionItem = _contentManager.Get(viewId, VersionOptions.Latest);
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryPartRecord = projectionPart.Record.QueryPartRecord;

            if (queryPartRecord.Layouts.Count == 0) return re;
            var properties = queryPartRecord.Layouts[0].Properties;
            re.AddRange(properties);
            return re;
        }
        private dynamic GetListModel(int viewId)
        {
          
            if (viewId == -1) return null;

            var contentItem = _contentManager.Get(viewId, VersionOptions.Latest);

            dynamic model = _contentManager.BuildDisplay(contentItem);
            return model;
        }

        public ActionResult Create(string id, int? containerId)
        {
            if (string.IsNullOrEmpty(id))
                return CreatableTypeList(containerId);
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            id = pluralService.Singularize(id);
            var contentItem = _contentManager.New(id);

            if (!Services.Authorizer.Authorize(Permissions.EditContent, contentItem, T("Cannot create content")))
                return new HttpUnauthorizedResult();

            if (containerId.HasValue && contentItem.Is<ContainablePart>())
            {
                var common = contentItem.As<CommonPart>();
                if (common != null)
                {
                    common.Container = _contentManager.Get(containerId.Value);
                }
            }

            dynamic model = _contentManager.BuildEditor(contentItem);
            var contentTypeDefinition = contentItem.TypeDefinition;
            string layout = contentTypeDefinition.Settings.ContainsKey("Layout")
                                ? contentTypeDefinition.Settings["Layout"]
                                : null;
            model.Layout = layout;

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        ActionResult CreatableTypeList(int? containerId)
        {
            dynamic viewModel = Shape.ViewModel(ContentTypes: GetCreatableTypes(containerId.HasValue), ContainerId: containerId);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View("CreatableTypeList", (object)viewModel);
        }

        private IEnumerable<ContentTypeDefinition> GetCreatableTypes(bool andContainable)
        {
            return _contentDefinitionManager.ListTypeDefinitions().Where(ctd => ctd.Settings.GetModel<ContentTypeSettings>().Creatable && (!andContainable || ctd.Parts.Any(p => p.PartDefinition.Name == "ContainablePart")));
        }


        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public ActionResult CreatePOST(string id, string returnUrl)
        {
            return CreatePOST(id, returnUrl, contentItem =>
            {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    _contentManager.Publish(contentItem);
            });
        }

        private ActionResult CreatePOST(string id, string returnUrl, Action<ContentItem> conditionallyPublish)
        {
            var contentItem = _contentManager.New(id);

            if (!Services.Authorizer.Authorize(Permissions.EditContent, contentItem, T("Couldn't create content")))
                return new HttpUnauthorizedResult();

            _contentManager.Create(contentItem, VersionOptions.Draft);

            dynamic model = _contentManager.UpdateEditor(contentItem, this);
            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            conditionallyPublish(contentItem);

            Services.Notifier.Information(string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                ? T("Your content has been created.")
                : T("Your {0} has been created.", contentItem.TypeDefinition.DisplayName));
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return this.RedirectLocal(returnUrl);
            }

            //return RedirectToAction("List", new {Id = id});
            var adminRouteValues = _contentManager.GetItemMetadata(contentItem).AdminRouteValues;
            return RedirectToRoute(adminRouteValues);
        }

        public ActionResult Edit(int id)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);

            if (contentItem == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.EditContent, contentItem, T("Cannot edit content")))
                return new HttpUnauthorizedResult();

            dynamic model = _contentManager.BuildEditor(contentItem);
            var contentTypeDefinition = contentItem.TypeDefinition;
            string layout = contentTypeDefinition.Settings.ContainsKey("Layout")
                                ? contentTypeDefinition.Settings["Layout"]
                                : null;
            model.Layout = layout;
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int id, string returnUrl)
        {
            return EditPOST(id, returnUrl, contentItem =>
            {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    _contentManager.Publish(contentItem);
            });
        }

        public ActionResult View(int id) {
            return View();
        }

        [HttpPost]
        public ActionResult Remove(int id, string returnUrl)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.Latest);

            if (!Services.Authorizer.Authorize(Permissions.DeleteContent, contentItem, T("Couldn't remove content")))
                return new HttpUnauthorizedResult();

            if (contentItem != null)
            {
                _contentManager.Remove(contentItem);
                Services.Notifier.Information(string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                    ? T("That content has been removed.")
                    : T("That {0} has been removed.", contentItem.TypeDefinition.DisplayName));
            }

            return this.RedirectLocal(returnUrl, () => RedirectToAction("List"));
        }

        private ActionResult EditPOST(int id, string returnUrl, Action<ContentItem> conditionallyPublish)
        {
            var contentItem = _contentManager.Get(id, VersionOptions.DraftRequired);

            if (contentItem == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.EditContent, contentItem, T("Couldn't edit content")))
                return new HttpUnauthorizedResult();

            string previousRoute = null;
            if (contentItem.Has<IAliasAspect>()
                && !string.IsNullOrWhiteSpace(returnUrl)
                && Request.IsLocalUrl(returnUrl)
                // only if the original returnUrl is the content itself
                && String.Equals(returnUrl, Url.ItemDisplayUrl(contentItem), StringComparison.OrdinalIgnoreCase)
                )
            {
                previousRoute = contentItem.As<IAliasAspect>().Path;
            }

            dynamic model = _contentManager.UpdateEditor(contentItem, this);
            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View("Edit", (object)model);
            }

            conditionallyPublish(contentItem);

            if (!string.IsNullOrWhiteSpace(returnUrl)
                && previousRoute != null
                && !String.Equals(contentItem.As<IAliasAspect>().Path, previousRoute, StringComparison.OrdinalIgnoreCase))
            {
                returnUrl = Url.ItemDisplayUrl(contentItem);
            }

            Services.Notifier.Information(string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                ? T("Your content has been saved.")
                : T("Your {0} has been saved.", contentItem.TypeDefinition.DisplayName));

           // return this.RedirectLocal(returnUrl, () => RedirectToAction("Edit", new RouteValueDictionary { { "Id", contentItem.Id } }));
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}