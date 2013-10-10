using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Data.Entity.Design.PluralizationServices;
using System.Web.Mvc;
using Coevery.Entities.Services;
using Coevery.Entities.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Utility.Extensions;
using Orchard.UI.Notify;
using EditTypeViewModel = Coevery.Entities.ViewModels.EditTypeViewModel;
using IContentDefinitionEditorEvents = Coevery.Entities.Settings.IContentDefinitionEditorEvents;
using IContentDefinitionService = Coevery.Entities.Services.IContentDefinitionService;

namespace Coevery.Entities.Controllers {
    public class SystemAdminController : Controller, IUpdateModel {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionEditorEvents _contentDefinitionEditorEvents;
        private readonly IContentMetadataService _contentMetadataService;

        public SystemAdminController(
            IOrchardServices orchardServices,
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionEditorEvents contentDefinitionEditorEvents,
            IContentMetadataService contentMetadataService) {
            Services = orchardServices;
            _contentDefinitionService = contentDefinitionService;
            T = NullLocalizer.Instance;
            _contentDefinitionEditorEvents = contentDefinitionEditorEvents;
            _contentMetadataService = contentMetadataService;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        #region Entity Methods

        public ActionResult List(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult Create() {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to create a content type."))) {
                return new HttpUnauthorizedResult();
            }

            var typeViewModel = _contentDefinitionService.GetType(string.Empty);

            return View(typeViewModel);
        }

        public ActionResult EntityName(string displayName, int version) {
            return Json(new {
                result = _contentMetadataService.ConstructEntityName(displayName.ToSafeName()),
                version = version
            });
        }

        public ActionResult Publish(string id) {
            var entity = _contentMetadataService.GetEntity(id);
            Services.ContentManager.Publish(entity.ContentItem);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePost(EditTypeViewModel viewModel) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to create a content type."))) {
                return new HttpUnauthorizedResult();
            }

            viewModel.DisplayName = string.IsNullOrWhiteSpace(viewModel.DisplayName) ? String.Empty : viewModel.DisplayName.Trim();
            viewModel.Name = (viewModel.Name ?? viewModel.DisplayName).ToSafeName();

            viewModel.FieldLabel = string.IsNullOrWhiteSpace(viewModel.FieldLabel) ? String.Empty : viewModel.FieldLabel.Trim();
            viewModel.FieldName = (viewModel.FieldName ?? viewModel.FieldLabel).ToSafeName();

            if (String.IsNullOrWhiteSpace(viewModel.DisplayName)) {
                ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.Name)) {
                ModelState.AddModelError("Name", T("The Content Type Id can't be empty.").ToString());
            }
            else if (!PluralizationService.CreateService(new CultureInfo("en-US")).IsSingular(viewModel.Name)) {
                ModelState.AddModelError("Name", T("The name should be singular.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.FieldLabel)) {
                ModelState.AddModelError("DisplayName", T("The Field Label name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.FieldName)) {
                ModelState.AddModelError("Name", T("The Field Name can't be empty.").ToString());
            }
            else if (!PluralizationService.CreateService(new CultureInfo("en-US")).IsSingular(viewModel.FieldName)) {
                ModelState.AddModelError("FieldName", T("The field name should be singular.").ToString());
            }

            if (!_contentMetadataService.CheckEntityCreationValid(viewModel.Name, viewModel.DisplayName)) {
                ModelState.AddModelError("Name", T("A type with the same Name or DisplayName already exists.").ToString());
            }

            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                var temp = (from values in ModelState
                    from error in values.Value.Errors
                    select error.ErrorMessage).ToArray();
                return Content(string.Concat(temp));
            }
            _contentMetadataService.CreateEntity(viewModel);
            return Json(new {entityName = viewModel.Name});
        }

        public ActionResult Edit(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type."))) {
                return new HttpUnauthorizedResult();
            }

            var entity = _contentMetadataService.GetEntity(id);

            if (entity == null) {
                return HttpNotFound();
            }
            var viewModel = new EditTypeViewModel {
                Name = entity.Name,
                DisplayName = entity.DisplayName
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(string id, EditTypeViewModel viewModel) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type."))) {
                return new HttpUnauthorizedResult();
            }
            var entity = _contentMetadataService.GetDraftEntity(id);

            if (entity == null) {
                return HttpNotFound();
            }
            bool valid = _contentMetadataService.CheckEntityDisplayValid(id, viewModel.DisplayName);
            if (!valid) {
                return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }
            entity.DisplayName = viewModel.DisplayName;
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Detail(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type."))) {
                return new HttpUnauthorizedResult();
            }

            var entity = _contentMetadataService.GetEntity(id);

            if (entity == null) {
                return HttpNotFound();
            }

            return View(new EntityDetailViewModel {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                HasPublished = entity.HasPublished(),
                PublishTip = "Works when the entity has been published."
            });
        }

        #endregion

        #region Field Methods

        public ActionResult CreateChooseType(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content part."))) {
                return new HttpUnauthorizedResult();
            }

            var viewModel = new AddFieldViewModel {
                Fields = _contentDefinitionService.GetFields().OrderBy(x => x.TemplateName),
            };

            return View(viewModel);
        }

        public ActionResult CreateEditInfo(string id, string fieldTypeName) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content part."))) {
                return new HttpUnauthorizedResult();
            }
            var contentFieldDefinition = new ContentFieldDefinition(fieldTypeName);
            var definition = new ContentPartFieldDefinition(contentFieldDefinition, string.Empty, new SettingsDictionary());
            var templates = _contentDefinitionEditorEvents.PartFieldEditor(definition);

            var viewModel = new AddFieldViewModel {
                FieldTypeName = fieldTypeName,
                TypeTemplates = templates,
                AddInLayout = true
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("CreateEditInfo")]
        public ActionResult CreateEditInfoPost(string id, AddFieldViewModel viewModel) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content part."))) {
                return new HttpUnauthorizedResult();
            }

            var entity = _contentMetadataService.GetDraftEntity(id);
            if (entity == null) {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Content(string.Format("The entity with name \"{0}\" doesn't exist!", id));
            }

            viewModel.DisplayName = string.IsNullOrWhiteSpace(viewModel.DisplayName)
                ? String.Empty : viewModel.DisplayName.Trim();
            viewModel.Name = (viewModel.Name ?? viewModel.DisplayName).ToSafeName();

            if (String.IsNullOrWhiteSpace(viewModel.DisplayName)) {
                ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.Name)) {
                ModelState.AddModelError("Name", T("The Technical Name can't be empty.").ToString());
            }

            if (viewModel.Name.ToLower() == "id") {
                ModelState.AddModelError("Name", T("The Field Name can't be any case of 'Id'.").ToString());
            }

            if (!_contentMetadataService.CheckFieldCreationValid(entity, viewModel.Name, viewModel.DisplayName)) {
                ModelState.AddModelError("Name", T("A field with the same name or displayName already exists.").ToString());
            }

            try {
                _contentMetadataService.CreateField(entity, viewModel, this);
                if (!ModelState.IsValid) {
                    Services.TransactionManager.Cancel();
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    var errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                        .Select(m => m.ErrorMessage).ToArray();
                    return Content(string.Concat(errors));
                }
            }
            catch (Exception ex) {
                var message = T("The \"{0}\" field was not added. {1}", viewModel.DisplayName, ex.Message);
                Services.Notifier.Information(message);
                Services.TransactionManager.Cancel();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message.ToString());
            }

            Services.Notifier.Information(T("The \"{0}\" field has been added.", viewModel.DisplayName));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult FieldName(bool creatingEntity, string entityName, string displayName, int version) {
            if (creatingEntity) {
                return Json(new {
                    result = displayName.ToSafeName(),
                    version = version
                });
            }
            return Json(new {
                result = _contentMetadataService.ConstructFieldName(entityName, displayName.ToSafeName()),
                version = version
            });
        }

        public ActionResult EditFields(string id, string fieldName) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type."))) {
                return new HttpUnauthorizedResult();
            }
            var entity = _contentMetadataService.GetEntity(id);
            if (entity == null) {
                return HttpNotFound();
            }
            var field = entity.FieldMetadataRecords.FirstOrDefault(x => x.Name == fieldName);
            if (field == null) {
                return HttpNotFound();
            }
            var settings = _contentMetadataService.ParseSetting(field.Settings);
            var fieldDefinition = new ContentFieldDefinition(field.ContentFieldDefinitionRecord.Name);
            var viewModel = new EditPartFieldViewModel {
                Name = field.Name,
                DisplayName = settings["DisplayName"],
                Settings = settings,
                FieldDefinition = new EditFieldViewModel(fieldDefinition),
                Templates = _contentDefinitionEditorEvents.PartFieldEditor(new ContentPartFieldDefinition(fieldDefinition, field.Name, settings))
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("EditFields")]
        public ActionResult EditFieldsPost(EditPartFieldViewModel viewModel, string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type."))) {
                return new HttpUnauthorizedResult();
            }

            var entity = _contentMetadataService.GetDraftEntity(id);
            if (viewModel == null || entity == null) {
                return HttpNotFound();
            }
            var field = entity.FieldMetadataRecords.FirstOrDefault(x => x.Name == viewModel.Name);
            if (field == null) {
                return HttpNotFound();
            }

            // prevent null reference exception in validation
            viewModel.DisplayName = viewModel.DisplayName ?? String.Empty;

            // remove extra spaces
            viewModel.DisplayName = viewModel.DisplayName.Trim();

            if (String.IsNullOrWhiteSpace(viewModel.DisplayName)) {
                ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
            }

            bool displayNameExist = entity.FieldMetadataRecords.Any(t => {
                string displayName = _contentMetadataService.ParseSetting(t.Settings)["DisplayName"];
                return t.Name != viewModel.Name && String.Equals(displayName.Trim(), viewModel.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase);
            });
            if (displayNameExist) {
                ModelState.AddModelError("DisplayName", T("A field with the same Display Name already exists.").ToString());
            }
            _contentMetadataService.UpdateField(field, viewModel.DisplayName, this);
            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                var temp = (from values in ModelState
                    from error in values.Value.Errors
                    select error.ErrorMessage).ToArray();
                return Content(string.Concat(temp));
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Fields() {
            return View();
        }

        #endregion

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        public ActionResult DependencyList(string id) {
            return View();
        }

        public ActionResult CreateDependency(string id) {
            var typeViewModel = _contentDefinitionService.GetType(id);
            var controlFields = new List<EditPartFieldViewModel>();
            var dependentFields = new List<EditPartFieldViewModel>();
            foreach (var field in typeViewModel.Fields) {
                switch (field.FieldDefinition.Name) {
                    case "OptionSetField":
                        controlFields.Add(field);
                        dependentFields.Add(field);
                        break;
                    case "BooleanField":
                        controlFields.Add(field);
                        break;
                }
            }
            var viewModel = new FieldDependencyViewModel {
                ControlFields = controlFields,
                DependentFields = dependentFields
            };
            return View(viewModel);
        }

        public ActionResult EditDependency(string entityName, int itemId) {
            var typeViewModel = _contentDefinitionService.GetType(entityName);
            return View();
        }
    }
}