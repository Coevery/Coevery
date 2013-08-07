using System;
using System.Linq;
using Coevery.Core.Services;
using Coevery.Fields.Settings;
using Coevery.Fields.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Utility.Extensions;

namespace Coevery.Fields.Services {
    public class FieldService : Component, IFieldService {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly ISchemaUpdateService _schemaUpdateService;

        public FieldService(
            IContentDefinitionService contentDefinitionService,
            ISchemaUpdateService schemaUpdateService) {
            _contentDefinitionService = contentDefinitionService;
            _schemaUpdateService = schemaUpdateService;
        }

        public bool Create(string entityName, AddFieldViewModel viewModel, IUpdateModel updateModel) {
            viewModel.DisplayName = viewModel.DisplayName ?? String.Empty;
            viewModel.DisplayName = viewModel.DisplayName.Trim();
            viewModel.Name = (viewModel.Name ?? viewModel.DisplayName).ToSafeName();

            bool hasError = false;
            if (String.IsNullOrWhiteSpace(viewModel.DisplayName)) {
                updateModel.AddModelError("DisplayName", T("The Display Name name can't be empty."));
                hasError = true;
            }

            if (String.IsNullOrWhiteSpace(viewModel.Name)) {
                updateModel.AddModelError("Name", T("The Technical Name can't be empty."));
                hasError = true;
            }

            if (viewModel.Name.ToLower() == "id") {
                updateModel.AddModelError("Name", T("The Field Name can't be any case of 'Id'."));
                hasError = true;
            }

            if (_contentDefinitionService.GetPart(entityName).Fields.Any(t => String.Equals(t.Name.Trim(), viewModel.Name.Trim(), StringComparison.OrdinalIgnoreCase))) {
                updateModel.AddModelError("Name", T("A field with the same name already exists."));
                hasError = true;
            }

            if (!String.IsNullOrWhiteSpace(viewModel.Name) && !viewModel.Name[0].IsLetter()) {
                updateModel.AddModelError("Name", T("The technical name must start with a letter."));
                hasError = true;
            }

            if (!String.Equals(viewModel.Name, viewModel.Name.ToSafeName(), StringComparison.OrdinalIgnoreCase)) {
                updateModel.AddModelError("Name", T("The technical name contains invalid characters."));
                hasError = true;
            }

            if (_contentDefinitionService.GetPart(entityName).Fields.Any(t => String.Equals(t.DisplayName.Trim(), Convert.ToString(viewModel.DisplayName).Trim(), StringComparison.OrdinalIgnoreCase))) {
                updateModel.AddModelError("DisplayName", T("A field with the same Display Name already exists."));
                hasError = true;
            }

            var prefix = viewModel.FieldTypeName + "Settings";
            var clientSettings = new FieldSettings();
            updateModel.TryUpdateModel(clientSettings, prefix, null, null);
            if (clientSettings.IsSystemField) {
                updateModel.AddModelError("IsSystemField", T("Can't modify the IsSystemField field."));
                hasError = true;
            }

            if (!hasError) {
                try {
                    _contentDefinitionService.AddFieldToPart(viewModel.Name, viewModel.DisplayName, viewModel.FieldTypeName, entityName, updateModel);
                    _schemaUpdateService.CreateColumn(entityName, viewModel.Name, viewModel.FieldTypeName);
                }
                catch (Exception ex) {
                    hasError = true;
                }
            }
            return !hasError;
        }
    }
}