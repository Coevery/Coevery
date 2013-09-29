using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Coevery.Relationship.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Projections.Models;
using Orchard.Localization;

namespace Coevery.Relationship.Settings {
    public class ReferenceFieldEditorEvents : FieldEditorEvents {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentManager _contentManager;
        private readonly IRelationshipService _relationshipService;
        public Localizer T { get; set; }

        public ReferenceFieldEditorEvents(
            IContentDefinitionService contentDefinitionService,
            IContentManager contentManager,
            IRelationshipService relationshipService) {
            _contentDefinitionService = contentDefinitionService;
            _contentManager = contentManager;
            _relationshipService = relationshipService;
            T = NullLocalizer.Instance;
        }

        public override IEnumerable<TemplateViewModel> FieldDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Reference", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "ReferenceField") {
                return;
            }
            var model = new ReferenceFieldSettings();
            if (updateModel.TryUpdateModel(model, "ReferenceFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "ReferenceFieldSettings");
                settingsDictionary["ReferenceFieldSettings.ContentTypeName"] = model.ContentTypeName;
                settingsDictionary["ReferenceFieldSettings.RelationshipName"] = model.RelationshipName;
                settingsDictionary["ReferenceFieldSettings.DisplayAsLink"] = model.DisplayAsLink.ToString();
                settingsDictionary["ReferenceFieldSettings.QueryId"] = model.QueryId.ToString("D");
                settingsDictionary["ReferenceFieldSettings.RelationshipId"] = model.RelationshipId.ToString("D");
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "ReferenceField") {
                return;
            }

            var model = settingsDictionary.TryGetModel<ReferenceFieldSettings>();
            if (model != null) {
                if (model.QueryId <= 0) {
                    model.QueryId = CreateQuery(model.ContentTypeName.ToString(CultureInfo.InvariantCulture));
                }

                if (model.RelationshipId <= 0) {
                    var entityName = settingsDictionary["EntityName"];
                    model.RelationshipId = _relationshipService.CreateOneToManyRelationship(builder.Name, model.RelationshipName, model.ContentTypeName, entityName);
                }

                UpdateSettings(model, builder, "ReferenceFieldSettings");
                builder.WithSetting("ReferenceFieldSettings.DisplayAsLink", model.DisplayAsLink.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("ReferenceFieldSettings.ContentTypeName", model.ContentTypeName.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("ReferenceFieldSettings.RelationshipName", model.RelationshipName.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("ReferenceFieldSettings.RelationshipId", model.RelationshipId.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("ReferenceFieldSettings.QueryId", model.QueryId.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "ReferenceField" ||
                definition.FieldDefinition.Name == "ReferenceFieldCreate") {
                var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
                var model = definition.Settings.GetModel<ReferenceFieldSettings>();
                model.ContentTypeList = metadataTypes.Select(item => new SelectListItem {
                    Text = item.Name,
                    Value = item.Name,
                    Selected = item.Name == model.ContentTypeName
                }).ToList();
                yield return DefinitionTemplate(model);
            }
        }

        private int CreateQuery(string entityType) {
            var queryPart = _contentManager.New<QueryPart>("Query");
            var filterGroup = new FilterGroupRecord();
            queryPart.Record.FilterGroups.Add(filterGroup);
            var filterRecord = new FilterRecord {
                Category = "Content",
                Type = "ContentTypes",
                Position = filterGroup.Filters.Count,
                State = GetContentTypeFilterState(entityType)
            };
            filterGroup.Filters.Add(filterRecord);
            _contentManager.Create(queryPart);
            return queryPart.Id;
        }

        private static string GetContentTypeFilterState(string entityType) {
            const string format = @"<Form><Description></Description><ContentTypes>{0}</ContentTypes></Form>";
            return string.Format(format, entityType);
        }
    }
}