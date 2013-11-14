using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Coevery.Relationship.Records;
using Coevery.Relationship.Services;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;
using Coevery.Data;
using Coevery.Localization;

namespace Coevery.Relationship.Settings {
    public class ReferenceFieldEditorEvents : FieldEditorEvents {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IRelationshipService _relationshipService;
        private readonly IRepository<OneToManyRelationshipRecord> _repository;

        public Localizer T { get; set; }

        public ReferenceFieldEditorEvents(
            IContentDefinitionService contentDefinitionService,
            IRelationshipService relationshipService,
            IRepository<OneToManyRelationshipRecord> repository) {
            _contentDefinitionService = contentDefinitionService;
            _relationshipService = relationshipService;
            _repository = repository;
            T = NullLocalizer.Instance;
        }

        public override IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Reference", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "ReferenceField") {
                return;
            }

            var model = new ReferenceFieldSettings();
            if (updateModel.TryUpdateModel(model, "ReferenceFieldSettings", null, null)) {
                if (string.IsNullOrEmpty(model.ContentTypeName)) {
                    throw new Exception("primary entity is null", new ArgumentNullException());
                }
                if (model.QueryId <= 0) {
                    model.QueryId = _relationshipService.CreateEntityQuery(model.ContentTypeName);
                }

                if (model.RelationshipId <= 0) {
                    var entityName = settingsDictionary["EntityName"];
                    model.RelationshipId = _relationshipService.CreateOneToManyRelationship(fieldName, model.RelationshipName, model.ContentTypeName, entityName);
                }

                UpdateSettings(model, settingsDictionary, "ReferenceFieldSettings");
                settingsDictionary["ReferenceFieldSettings.ContentTypeName"] = model.ContentTypeName;
                settingsDictionary["ReferenceFieldSettings.RelationshipName"] = model.RelationshipName;
                settingsDictionary["ReferenceFieldSettings.DisplayAsLink"] = model.DisplayAsLink.ToString();
                settingsDictionary["ReferenceFieldSettings.QueryId"] = model.QueryId.ToString(CultureInfo.InvariantCulture);
                settingsDictionary["ReferenceFieldSettings.RelationshipId"] = model.RelationshipId.ToString(CultureInfo.InvariantCulture);
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "ReferenceField") {
                return;
            }

            var model = settingsDictionary.TryGetModel<ReferenceFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "ReferenceFieldSettings");
                builder.WithSetting("ReferenceFieldSettings.IsDisplayField", model.IsDisplayField.ToString());
                builder.WithSetting("ReferenceFieldSettings.DisplayAsLink", model.DisplayAsLink.ToString());
                builder.WithSetting("ReferenceFieldSettings.ContentTypeName", model.ContentTypeName);
                builder.WithSetting("ReferenceFieldSettings.RelationshipName", model.RelationshipName);
                builder.WithSetting("ReferenceFieldSettings.RelationshipId", model.RelationshipId.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("ReferenceFieldSettings.QueryId", model.QueryId.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override void FieldDeleted(string fieldType, string fieldName, SettingsDictionary settingsDictionary) {
            if (fieldType != "ReferenceField") {
                return;
            }
            var relationshipName = settingsDictionary["ReferenceFieldSettings.RelationshipName"];
            var record = _repository.Table
                .FirstOrDefault(x => x.Relationship.Name == relationshipName
                                     && x.Relationship.RelatedEntity.ContentItemVersionRecord.Latest);
            if (record != null) {
                _relationshipService.DeleteRelationship(record);
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
    }
}