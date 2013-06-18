using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Coevery.Fields.Records;
using Coevery.Fields.Settings;
using Coevery.Metadata.Services;
using Coevery.Metadata.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard;
using Orchard.ContentManagement.ViewModels;
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Orchard.Localization;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;

namespace Coevery.Metadata.Controllers {
    public class FieldViewTemplateController : Controller, IUpdateModel {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentDefinitionEditorEvents _contentDefinitionEditorEvents;
        private readonly IRepository<OptionItemRecord> _optionItemRepository;
        private readonly IRepository<ContentPartFieldDefinitionRecord> _partFieldDefinitionRepository;

        public FieldViewTemplateController(
            IOrchardServices orchardServices,
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionManager contentDefinitionManager,
            IContentDefinitionEditorEvents contentDefinitionEditorEvents,
            IRepository<OptionItemRecord> optionItemRepository,
            IRepository<ContentPartFieldDefinitionRecord> partFieldDefinitionRepository) {
            Services = orchardServices;
            _contentDefinitionService = contentDefinitionService;
            _contentDefinitionManager = contentDefinitionManager;
            _contentDefinitionEditorEvents = contentDefinitionEditorEvents;
            _optionItemRepository = optionItemRepository;
            _partFieldDefinitionRepository = partFieldDefinitionRepository;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }

        public ActionResult List() {
            return View();
        }

        public ActionResult Edit(string id, string subId) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type.")))
                return new HttpUnauthorizedResult();

            //var fieldDefinition = _partFieldDefinitionRepository.Get(f => f.Name == "Sport");
            //var item = new OptionItemRecord {
            //    Value = "Baseball",
            //    ContentPartFieldDefinitionRecord = fieldDefinition
            //};
            //_optionItemRepository.Create(item);

            var items = _optionItemRepository.Table
                .Where(x => x.ContentPartFieldDefinitionRecord.Name == "Sport")
                .Select(x => x);

            var typeViewModel = _contentDefinitionService.GetType(id);
            if (typeViewModel == null) {
                return HttpNotFound();
            }

            var fieldViewModel = typeViewModel.Fields.FirstOrDefault(x => x.Name == subId);

            if (fieldViewModel == null) {
                return HttpNotFound();
            }

            return View(fieldViewModel);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPost(EditPartFieldViewModel viewModel, string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type.")))
                return new HttpUnauthorizedResult();

            if (viewModel == null)
                return HttpNotFound();

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

            var typeViewModel = _contentDefinitionService.GetType(id);
            var field = typeViewModel.Fields.FirstOrDefault(f => f.Name == viewModel.Name);
            if (field == null) {
                return HttpNotFound();
            }

            CheckData(field);
            if (!ModelState.IsValid) {
                string displayName = viewModel.DisplayName;
                viewModel = typeViewModel.Fields.FirstOrDefault(x => x.Name == viewModel.Name);
                viewModel.DisplayName = displayName;
                return View(viewModel);
            }

            field.DisplayName = viewModel.DisplayName;
            _contentDefinitionManager.StorePartDefinition(partViewModel._Definition);

            _contentDefinitionService.AlterField(typeViewModel.Name, viewModel, this);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Create(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content part.")))
                return new HttpUnauthorizedResult();

            //var partViewModel = _contentDefinitionService.GetPart(id);

            //if (partViewModel == null)
            //{
            //    //id passed in might be that of a type w/ no implicit field
            //    var typeViewModel = _contentDefinitionService.GetType(id);
            //    if (typeViewModel != null)
            //        partViewModel = new EditPartViewModel(new ContentPartDefinition(id));
            //    else
            //        return HttpNotFound();
            //}

            var viewModel = new AddFieldViewModel {
                Fields = _contentDefinitionService.GetFields().OrderBy(x => x.FieldTypeName),
            };

            return View(viewModel);
        }

        public ActionResult DependencyList(string id) {
            var typeViewModel = _contentDefinitionService.GetType(id);
            var controlFields = new List<EditPartFieldViewModel>();
            var dependentFields = new List<EditPartFieldViewModel>();
            foreach (var field in typeViewModel.Fields) {
                switch (field.FieldDefinition.Name) {
                    case "SelectField":
                        controlFields.Add(field);
                        dependentFields.Add(field);
                        break;
                    case "MultiSelectField":
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

        public ActionResult EditFieldInfo(string id, string subId) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content part.")))
                return new HttpUnauthorizedResult();

            var definition = new ContentPartFieldDefinition(new ContentFieldDefinition(subId), string.Empty, new SettingsDictionary());
            //var definition = new ContentPartFieldDefinition(new ContentFieldDefinition(subId + "Display"), string.Empty, new SettingsDictionary());
            var templates = _contentDefinitionEditorEvents.PartFieldEditor(definition);

            var viewModel = new AddFieldViewModel {
                FieldTypeName = subId,
                TypeTemplates = templates,
                AddInLayout = true
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("EditFieldInfo")]
        public ActionResult CreatePost(AddFieldViewModel viewModel, string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content part.")))
                return new HttpUnauthorizedResult();

            var partViewModel = _contentDefinitionService.GetPart(id);
            var typeViewModel = _contentDefinitionService.GetType(id);
            if (partViewModel == null) {
                // id passed in might be that of a type w/ no implicit field
                if (typeViewModel != null) {
                    partViewModel = new EditPartViewModel { Name = typeViewModel.Name };
                    _contentDefinitionService.AddPart(new CreatePartViewModel { Name = partViewModel.Name });
                    _contentDefinitionService.AddPartToType(partViewModel.Name, typeViewModel.Name);
                }
                else {
                    return HttpNotFound();
                }
            }

            viewModel.DisplayName = viewModel.DisplayName ?? String.Empty;
            viewModel.DisplayName = viewModel.DisplayName.Trim();
            viewModel.Name = viewModel.Name ?? String.Empty;

            if (String.IsNullOrWhiteSpace(viewModel.DisplayName)) {
                ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.Name)) {
                ModelState.AddModelError("Name", T("The Technical Name can't be empty.").ToString());
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

            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }

            try {
                _contentDefinitionService.AddFieldToPart(viewModel.Name, viewModel.DisplayName, viewModel.FieldTypeName, partViewModel.Name);
            }
            catch (Exception ex) {
                Services.Notifier.Information(T("The \"{0}\" field was not added. {1}", viewModel.DisplayName, ex.Message));
                Services.TransactionManager.Cancel();
                return Create(id);
            }

            var edit = new EditPartFieldViewModel {
                Name = viewModel.Name
            };
            _contentDefinitionService.AlterField(typeViewModel.Name, edit, this);
            typeViewModel = _contentDefinitionService.GetType(id);
            var field = typeViewModel.Fields.First(f => f.Name == viewModel.Name);
            CheckData(field);
            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }

            Services.Notifier.Information(T("The \"{0}\" field has been added.", viewModel.DisplayName));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        private void CheckData(EditPartFieldViewModel serverField) {
            var settingsStr = serverField.FieldDefinition.Name + "Settings.";
            var clientSettings = new FieldSettings();
            TryUpdateModel(clientSettings, "BooleanFieldSettings");

            var serverSettings = new FieldSettings {
                IsSystemField = bool.Parse(serverField.Settings[settingsStr + "IsSystemField"]),
                Required = bool.Parse(serverField.Settings[settingsStr + "Required"]),
                ReadOnly = bool.Parse(serverField.Settings[settingsStr + "ReadOnly"]),
                AlwaysInLayout = bool.Parse(serverField.Settings[settingsStr + "AlwaysInLayout"])
            };

            if (clientSettings.IsSystemField != serverSettings.IsSystemField) {
                ModelState.AddModelError("IsSystemField", T("Can't modify the IsSystemField field.").ToString());
            }

            if (serverSettings.IsSystemField) {
                if (clientSettings.Required != serverSettings.Required) {
                    ModelState.AddModelError("Required", T("Can't modify the Required field.").ToString());
                }
                if (clientSettings.ReadOnly != serverSettings.ReadOnly) {
                    ModelState.AddModelError("ReadOnly", T("Can't modify the ReadOnly field.").ToString());
                }
                if (clientSettings.AlwaysInLayout != serverSettings.AlwaysInLayout) {
                    ModelState.AddModelError("AlwaysInLayout", T("Can't modify the AlwaysInLayout field.").ToString());
                }
            }
        }
    }
}
