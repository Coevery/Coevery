using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Coevery.Entities.ViewModels;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Localization;
using Coevery.Logging;
using Coevery.Utility.Extensions;
using Coevery.UI.Notify;
using EditTypeViewModel = Coevery.Entities.ViewModels.EditTypeViewModel;
using IContentDefinitionEditorEvents = Coevery.Entities.Settings.IContentDefinitionEditorEvents;
using IContentDefinitionService = Coevery.Entities.Services.IContentDefinitionService;

namespace Coevery.Entities.Controllers {
    public class SystemAdminController : Controller, IUpdateModel {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionEditorEvents _contentDefinitionEditorEvents;
        private readonly IContentMetadataService _contentMetadataService;
        private readonly ISettingService _settingService;
        private readonly IEntityRecordEditorEvents _entityRecordEditorEvents;

        public SystemAdminController(
            ICoeveryServices coeveryServices,
            ISettingService settingService,
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionEditorEvents contentDefinitionEditorEvents,
            IContentMetadataService contentMetadataService,
            IEntityRecordEditorEvents entityRecordEditorEvents) {
            Services = coeveryServices;
            _settingService = settingService;
            _contentDefinitionService = contentDefinitionService;
            T = NullLocalizer.Instance;
            _contentDefinitionEditorEvents = contentDefinitionEditorEvents;
            _contentMetadataService = contentMetadataService;
            _entityRecordEditorEvents = entityRecordEditorEvents;
        }

        public ICoeveryServices Services { get; private set; }
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
            var entityRecords = _entityRecordEditorEvents.FieldSettingsEditor().ToList();
            var fieldTypes = entityRecords.Select(x =>
                new SelectListItem {Text = x.FieldTypeDisplayName, Value = x.FieldTypeName});

            var typeViewModel = _contentDefinitionService.GetType(string.Empty);
            typeViewModel.Settings.Add("CollectionName", string.Empty);
            typeViewModel.Settings.Add("CollectionDisplayName", string.Empty);
            typeViewModel.FieldTypes = fieldTypes;
            typeViewModel.FieldTemplates = entityRecords;

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

            if (String.IsNullOrWhiteSpace(viewModel.FieldLabel)) {
                ModelState.AddModelError("DisplayName", T("The Field Label name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.FieldName)) {
                ModelState.AddModelError("Name", T("The Field Name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.FieldType))
            {
                ModelState.AddModelError("Name", T("The FieldType can't be empty.").ToString());
            }

            if (!_contentMetadataService.CheckEntityCreationValid(viewModel.Name, viewModel.DisplayName, viewModel.Settings)) {
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

            _contentMetadataService.CreateEntity(viewModel, this);
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
                DisplayName = entity.DisplayName,
                Settings = entity.EntitySetting
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
            bool valid = _contentMetadataService.CheckEntityDisplayValid(id, viewModel.DisplayName, viewModel.Settings);
            if (!valid) {
                return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }
            entity.DisplayName = viewModel.DisplayName;
            entity.EntitySetting = viewModel.Settings;
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

            var viewModel = new EntityDetailViewModel {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                HasPublished = entity.HasPublished(),
                Settings = entity.EntitySetting
            };

            return View(viewModel);
        }

        #endregion

        #region Field Methods

        public ActionResult ChooseFieldType(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content part."))) {
                return new HttpUnauthorizedResult();
            }

            var viewModel = new AddFieldViewModel {
                Fields = _contentDefinitionService.GetFields().OrderBy(x => x.TemplateName),
            };

            return View(viewModel);
        }

        public ActionResult FillFieldInfo(string id, string fieldTypeName) {
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

        public ActionResult ConfirmFieldInfo(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content part."))) {
                return new HttpUnauthorizedResult();
            }
            return View();
        }

        [HttpPost, ActionName("FillFieldInfo")]
        public ActionResult FillFieldInfoPost(string id, AddFieldViewModel viewModel) {
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
            var settings = _settingService.ParseSetting(field.Settings);
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
                string displayName = _settingService.ParseSetting(t.Settings)["DisplayName"];
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