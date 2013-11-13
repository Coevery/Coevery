using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Coevery.Entities.ViewModels;
using Coevery.Relationship.Services;
using Coevery.Relationship.Settings;

namespace Coevery.Relationship.EntityRecords {
    public class ReferenceFieldEntityRecordEditorEvents : EntityRecordEditorEventsBase {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IRelationshipService _relationshipService;

        public ReferenceFieldEntityRecordEditorEvents(
            IContentDefinitionService contentDefinitionService,
            IRelationshipService relationshipService) {
            _contentDefinitionService = contentDefinitionService;
            _relationshipService = relationshipService;
        }

        public override IEnumerable<EntityRecordViewModel> FieldSettingsEditor() {
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
            var model = new ReferenceFieldSettings {
                ContentTypeList = metadataTypes.Select(item => new SelectListItem {
                    Text = item.Name,
                    Value = item.Name,
                }).ToList()
            };

            var templateViewModel = new TemplateViewModel(model) {
                TemplateName = "DefinitionTemplates/ReferenceFieldEntityRecord"
            };
            yield return new EntityRecordViewModel {
                FieldTypeName = "ReferenceField",
                FieldTypeDisplayName = "Reference",
                TemplateViewModel = templateViewModel
            };
        }

        public override void FieldSettingsEditorUpdate(string fieldType, string fieldName, SettingsDictionary settings, IUpdateModel updateModel) {
            if (fieldType != "ReferenceField") {
                return;
            }

            var model = new ReferenceFieldSettings();
            if (updateModel.TryUpdateModel(model, "ReferenceFieldSettings", null, null)) {
                var queryId = _relationshipService.CreateEntityQuery(model.ContentTypeName);
                var entityName = settings["EntityName"];
                var relationshipId = _relationshipService.CreateOneToManyRelationship(fieldName, model.RelationshipName, model.ContentTypeName, entityName);

                settings["ReferenceFieldSettings.QueryId"] = queryId.ToString(CultureInfo.InvariantCulture);
                settings["ReferenceFieldSettings.RelationshipId"] = relationshipId.ToString(CultureInfo.InvariantCulture);
                settings["ReferenceFieldSettings.ContentTypeName"] = model.ContentTypeName;
                settings["ReferenceFieldSettings.RelationshipName"] = model.RelationshipName;
                settings["ReferenceFieldSettings.DisplayAsLink"] = bool.FalseString;
                settings["ReferenceFieldSettings.IsDisplayField"] = bool.TrueString;
                settings["ReferenceFieldSettings.HelpText"] = string.Empty;
                settings["ReferenceFieldSettings.Required"] = bool.TrueString;
                settings["ReferenceFieldSettings.ReadOnly"] = bool.TrueString;
                settings["ReferenceFieldSettings.AlwaysInLayout"] = bool.TrueString;
                settings["ReferenceFieldSettings.IsSystemField"] = bool.TrueString;
                settings["ReferenceFieldSettings.IsAudit"] = bool.FalseString;
            }
        }
    }
}