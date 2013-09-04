using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Data.Entity.Design.PluralizationServices;
using System.Web.Mvc;
using Coevery.Core.Services;
using Coevery.Entities.Events;
using Coevery.Entities.Settings;
using Coevery.Entities.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Utility.Extensions;
using Orchard.UI.Notify;
using Orchard.Mvc;
using EditTypeViewModel = Coevery.Entities.ViewModels.EditTypeViewModel;
using IContentDefinitionEditorEvents = Orchard.ContentManagement.MetaData.IContentDefinitionEditorEvents;
using IContentDefinitionService = Coevery.Entities.Services.IContentDefinitionService;

namespace Coevery.Entities.Controllers {
    public class SystemAdminController : Controller, IUpdateModel {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentDefinitionEditorEvents _contentDefinitionEditorEvents;
        private readonly IEntityEvents _entityEvents;
        private readonly IFieldEvents _fieldEvents;

        public SystemAdminController(
            IOrchardServices orchardServices,
            IContentDefinitionService contentDefinitionService,
            ISchemaUpdateService schemaUpdateService,
            IContentDefinitionManager contentDefinitionManager,
            IContentDefinitionEditorEvents contentDefinitionEditorEvents,
            IEntityEvents entityEvents,
            IFieldEvents fieldEvents) {
            Services = orchardServices;
            _contentDefinitionService = contentDefinitionService;
            _schemaUpdateService = schemaUpdateService;
            T = NullLocalizer.Instance;
            _contentDefinitionManager = contentDefinitionManager;
            _contentDefinitionEditorEvents = contentDefinitionEditorEvents;
            _entityEvents = entityEvents;
            _fieldEvents = fieldEvents;
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
                result = _contentDefinitionService.GenerateContentTypeNameFromDisplayName(displayName),
                version = version
            });
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(EditTypeViewModel viewModel) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to create a content type."))) {
                return new HttpUnauthorizedResult();
            }

            viewModel.DisplayName = viewModel.DisplayName ?? String.Empty;
            viewModel.Name = (viewModel.Name ?? viewModel.DisplayName).ToSafeName();

            viewModel.FieldLabel = viewModel.FieldLabel ?? String.Empty;
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

            if (!PluralizationService.CreateService(new CultureInfo("en-US")).IsSingular(viewModel.Name)) {
                ModelState.AddModelError("Name", T("The name should be singular.").ToString());
            }

            if (_contentDefinitionService.GetTypes().Any(t => String.Equals(t.Name.Trim(), viewModel.Name.Trim(), StringComparison.OrdinalIgnoreCase))) {
                ModelState.AddModelError("Name", T("A type with the same Id already exists.").ToString());
            }

            if (!String.IsNullOrWhiteSpace(viewModel.Name) && !viewModel.Name[0].IsLetter()) {
                ModelState.AddModelError("Name", T("The technical name must start with a letter.").ToString());
            }

            if (_contentDefinitionService.GetTypes().Any(t => String.Equals(t.DisplayName.Trim(), viewModel.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase))) {
                ModelState.AddModelError("DisplayName", T("A type with the same Display Name already exists.").ToString());
            }

            if (!PluralizationService.CreateService(new CultureInfo("en-US")).IsSingular(viewModel.FieldName)) {
                ModelState.AddModelError("FieldName", T("The field name should be singular.").ToString());
            }

            if (!String.IsNullOrWhiteSpace(viewModel.FieldName) && !viewModel.FieldName[0].IsLetter()) {
                ModelState.AddModelError("FieldName", T("The technical field name must start with a letter.").ToString());
            }

            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var temp = (from values in ModelState
                            from error in values.Value.Errors
                            select error.ErrorMessage).ToArray();
                return Content(string.Concat(temp));
            }

            _contentDefinitionService.AlterType(viewModel, this);
            _contentDefinitionService.AddPartToType(viewModel.Name, viewModel.Name);

            _contentDefinitionManager.AlterPartDefinition(viewModel.Name,
                builder => builder.WithField(viewModel.FieldName,
                    fieldBuilder => fieldBuilder
                        .OfType("CoeveryTextField")
                        .WithDisplayName(viewModel.FieldLabel)
                        .WithSetting("CoeveryTextFieldSettings.IsDispalyField", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.Required", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.ReadOnly", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.AlwaysInLayout", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.IsSystemField", bool.TrueString)
                        .WithSetting("CoeveryTextFieldSettings.IsAudit", bool.FalseString)
                        .WithSetting("CoeveryTextFieldSettings.HelpText", string.Empty)
                        .WithSetting("Storage", "Part")));

            //adds CommonPart by default
            //_contentDefinitionService.AddPartToType("CoeveryCommonPart", viewModel.Name);

            Services.Notifier.Information(T("The \"{0}\" content type has been created.", viewModel.DisplayName));
            _entityEvents.OnCreated(viewModel.Name);

            _schemaUpdateService.CreateTable(viewModel.Name,
                context => context.FieldColumn(viewModel.FieldName, "CoeveryTextField"));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Edit(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type."))) {
                return new HttpUnauthorizedResult();
            }

            var typeViewModel = _contentDefinitionService.GetType(id);

            if (typeViewModel == null) {
                return HttpNotFound();
            }

            return View(typeViewModel);
        }

        public ActionResult Detail(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type."))) {
                return new HttpUnauthorizedResult();
            }

            var typeViewModel = _contentDefinitionService.GetType(id);

            if (typeViewModel == null) {
                return HttpNotFound();
            }

            return View(typeViewModel);
        }

        #endregion

        #region Field Methods
        public ActionResult CreateChooseType(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content part."))) {
                return new HttpUnauthorizedResult();
            }

            var viewModel = new AddFieldViewModel {
                Fields = _contentDefinitionService.GetFields().OrderBy(x => x.FieldTypeName),
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

            var partViewModel = _contentDefinitionService.GetPart(id);
            var typeViewModel = _contentDefinitionService.GetType(id);

            if (partViewModel == null) {
                // id passed in might be that of a type w/ no implicit field
                if (typeViewModel != null) {
                    partViewModel = new EditPartViewModel {Name = typeViewModel.Name};
                    _contentDefinitionService.AddPart(new CreatePartViewModel {Name = partViewModel.Name});
                    _contentDefinitionService.AddPartToType(partViewModel.Name, typeViewModel.Name);
                }
                else {
                    return HttpNotFound();
                }
            }

            viewModel.DisplayName = viewModel.DisplayName ?? String.Empty;
            viewModel.DisplayName = viewModel.DisplayName.Trim();
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

            if (_contentDefinitionService.GetPart(partViewModel.Name).Fields.Any(t => String.Equals(t.Name.Trim(), viewModel.Name.Trim(), StringComparison.OrdinalIgnoreCase))) {
                ModelState.AddModelError("Name", T("A field with the same name already exists.").ToString());
            }

            if (!String.IsNullOrWhiteSpace(viewModel.Name) && !viewModel.Name[0].IsLetter()) {
                ModelState.AddModelError("Name", T("The technical name must start with a letter.").ToString());
            }

            if (!String.Equals(viewModel.Name, viewModel.Name.ToSafeName(), StringComparison.OrdinalIgnoreCase)) {
                ModelState.AddModelError("Name", T("The technical name contains invalid characters.").ToString());
            }

            if (_contentDefinitionService.GetPart(partViewModel.Name).Fields.Any(t => String.Equals(t.DisplayName.Trim(), Convert.ToString(viewModel.DisplayName).Trim(), StringComparison.OrdinalIgnoreCase))) {
                ModelState.AddModelError("DisplayName", T("A field with the same Display Name already exists.").ToString());
            }

            try {
                _contentDefinitionService.AddFieldToPart(viewModel.Name, viewModel.DisplayName, viewModel.FieldTypeName, partViewModel.Name);
                _contentDefinitionService.AlterField(partViewModel.Name, viewModel.Name, this);

                if (!ModelState.IsValid) {
                    Services.TransactionManager.Cancel();
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    var errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                        .Select(m => m.ErrorMessage).ToArray();
                    return Content(string.Concat(errors));
                }

                _schemaUpdateService.CreateColumn(typeViewModel.Name, viewModel.Name, viewModel.FieldTypeName);
            }
            catch (Exception ex) {
                var message = T("The \"{0}\" field was not added. {1}", viewModel.DisplayName, ex.Message);
                Services.Notifier.Information(message);
                Services.TransactionManager.Cancel();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message.ToString());
            }
       
            _fieldEvents.OnCreated(id, viewModel.Name, viewModel.AddInLayout);
            Services.Notifier.Information(T("The \"{0}\" field has been added.", viewModel.DisplayName));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult FieldName(bool creatingEntity,string entityName,string displayName, int version) {
            if (creatingEntity) {
                return Json(new {
                    result = displayName.ToSafeName(),
                    version = version
                });
            }
            return Json(new {
                result = _contentDefinitionService.GenerateFieldNameFromDisplayName(entityName, displayName),
                version = version
            });
        }  

        public ActionResult EditFields(string id, string fieldName) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type."))) {
                return new HttpUnauthorizedResult();
            }

            var typeViewModel = _contentDefinitionService.GetType(id);
            if (typeViewModel == null) {
                return HttpNotFound();
            }

            var fieldViewModel = typeViewModel.Fields.FirstOrDefault(x => x.Name == fieldName);

            if (fieldViewModel == null) {
                return HttpNotFound();
            }

            return View(fieldViewModel);
        }

        [HttpPost, ActionName("EditFields")]
        public ActionResult EditFieldsPost(EditPartFieldViewModel viewModel, string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type."))) {
                return new HttpUnauthorizedResult();
            }

            if (viewModel == null) {
                return HttpNotFound();
            }

            var partViewModel = _contentDefinitionService.GetPart(id);
            if (partViewModel == null) {
                return HttpNotFound();
            }

            // prevent null reference exception in validation
            viewModel.DisplayName = viewModel.DisplayName ?? String.Empty;

            // remove extra spaces
            viewModel.DisplayName = viewModel.DisplayName.Trim();

            if (String.IsNullOrWhiteSpace(viewModel.DisplayName)) {
                ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
            }

            if (partViewModel.Fields.Any(t => t.Name != viewModel.Name && String.Equals(t.DisplayName.Trim(), viewModel.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase))) {
                ModelState.AddModelError("DisplayName", T("A field with the same Display Name already exists.").ToString());
            }

            var fieldDefinition = _contentDefinitionManager.GetPartDefinition(id).Fields.FirstOrDefault(f => f.Name == viewModel.Name);

            if (fieldDefinition == null) {
                return HttpNotFound();
            }

            var typeViewModel = _contentDefinitionService.GetType(id);
            var field = typeViewModel.Fields.FirstOrDefault(f => f.Name == viewModel.Name);
            CheckData(field);
            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                var temp = (from values in ModelState
                    from error in values.Value.Errors
                    select error.ErrorMessage).ToArray();
                return Content(string.Concat(temp));
            }

            fieldDefinition.DisplayName = viewModel.DisplayName;
            _contentDefinitionManager.StorePartDefinition(partViewModel._Definition);
            _contentDefinitionService.AlterField(id, viewModel.Name, this);
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

        private void CheckData(EditPartFieldViewModel serverField) {
            var prefix = serverField.FieldDefinition.Name + "Settings";
            var buildingSettings = new FieldSettings();
            TryUpdateModel(buildingSettings, prefix);

            var existingSetting = serverField.Settings.TryGetModel<FieldSettings>(prefix);

            if (buildingSettings.ReadOnly) {
                ModelState.AddModelError("ReadOnly", T("Can't modify the ReadOnly field.").ToString());
            }

            if (buildingSettings.IsSystemField != existingSetting.IsSystemField) {
                ModelState.AddModelError("IsSystemField", T("Can't modify the IsSystemField field.").ToString());
            }

            if (buildingSettings.IsSystemField) {
                if (buildingSettings.Required != existingSetting.Required) {
                    ModelState.AddModelError("Required", T("Can't modify the Required field.").ToString());
                }
                if (buildingSettings.AlwaysInLayout != existingSetting.AlwaysInLayout) {
                    ModelState.AddModelError("AlwaysInLayout", T("Can't modify the AlwaysInLayout field.").ToString());
                }
            }
        }
    }
}