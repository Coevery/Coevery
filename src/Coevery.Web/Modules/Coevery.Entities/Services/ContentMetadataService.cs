using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Common.Services;
using Coevery.Entities.Events;
using Coevery.Entities.Models;
using Coevery.Entities.Settings;
using Coevery.Entities.ViewModels;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Core.Settings.Metadata.Records;
using Coevery.Data;
using Coevery.Utility.Extensions;
using IContentDefinitionEditorEvents = Coevery.Entities.Settings.IContentDefinitionEditorEvents;

namespace Coevery.Entities.Services {
    public interface IContentMetadataService : IDependency {
        void CreateEntity(EditTypeViewModel sourceModel, IUpdateModel updateModel);
        IEnumerable<EntityMetadataPart> GetRawEntities();

        EntityMetadataPart GetEntity(int id);
        EntityMetadataPart GetEntity(string name);
        EntityMetadataPart GetDraftEntity(int id);
        EntityMetadataPart GetDraftEntity(string name);

        bool CheckEntityCreationValid(string name, string displayName, SettingsDictionary settings);
        bool CheckEntityPublished(string name);
        bool CheckEntityDisplayValid(string name, string displayName, SettingsDictionary settings);
        string ConstructEntityName(string entityName);
        string DeleteEntity(int id);

        IEnumerable<FieldMetadataRecord> GetFieldsList(int entityId);
        string ConstructFieldName(string entityName, string displayName);
        bool CheckFieldCreationValid(EntityMetadataPart entity, string name, string displayName);
        void CreateField(EntityMetadataPart entity, AddFieldViewModel viewModel, IUpdateModel updateModel);
        bool DeleteField(string filedName, string entityName);
        void UpdateField(FieldMetadataRecord record, string displayName, IUpdateModel updateModel);
    }

    public class ContentMetadataService : IContentMetadataService {
        private readonly ISettingService _settingService;
        private readonly IRepository<ContentFieldDefinitionRecord> _fieldDefinitionRepository;
        private readonly IContentDefinitionEditorEvents _contentDefinitionEditorEvents;
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IEntityEvents _entityEvents;
        private readonly IEntityRecordEditorEvents _entityRecordEditorEvents;

        public ContentMetadataService(
            ICoeveryServices services,
            ISettingService settingService,
            IContentDefinitionService contentDefinitionService,
            ISchemaUpdateService schemaUpdateService,
            IEntityEvents entityEvents,
            IRepository<ContentFieldDefinitionRecord> fieldDefinitionRepository,
            IContentDefinitionEditorEvents contentDefinitionEditorEvents,
            IEntityRecordEditorEvents entityRecordEditorEvents) {
            _contentDefinitionService = contentDefinitionService;
            _schemaUpdateService = schemaUpdateService;
            _entityEvents = entityEvents;
            _settingService = settingService;
            _fieldDefinitionRepository = fieldDefinitionRepository;
            _contentDefinitionEditorEvents = contentDefinitionEditorEvents;
            _entityRecordEditorEvents = entityRecordEditorEvents;
            Services = services;
        }

        public ICoeveryServices Services { get; private set; }

        #region Entity Related

        public IEnumerable<EntityMetadataPart> GetRawEntities() {
            return Services.ContentManager.Query<EntityMetadataPart, EntityMetadataRecord>().ForVersion(VersionOptions.Latest).OrderBy(d => d.Name).List();
        }

        //todo: use ModelState to check error in one function
        public bool CheckEntityCreationValid(string name, string displayName, SettingsDictionary settings) {
            var entities = Services.ContentManager.Query<EntityMetadataPart>(VersionOptions.Latest, "EntityMetadata").List();
            var isValid = !entities
                .Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)
                          || string.Equals(x.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));

            var collectionName = settings["CollectionName"];
            var collectionDisplayName = settings["CollectionDisplayName"];
            if (collectionName != collectionName.ToSafeName()) {
                isValid = false;
            }
            var hasDuplicateEntities = (from entity in entities
                let tSetting = entity.EntitySetting
                where tSetting != null
                      && (tSetting.ContainsKey("CollectionName") && tSetting["CollectionName"] == collectionName)
                      && (tSetting.ContainsKey("CollectionDisplayName") && tSetting["CollectionDisplayName"] == collectionDisplayName)
                select entity).Any();
            if (hasDuplicateEntities) {
                isValid = false;
            }
            return isValid;
        }

        public bool CheckEntityPublished(string name) {
            return GetEntity(name).HasPublished();
        }

        //todo: use ModelState to check error in one function
        public bool CheckEntityDisplayValid(string name, string displayName, SettingsDictionary settings) {
            var entities = Services.ContentManager
                .Query<EntityMetadataPart, EntityMetadataRecord>(VersionOptions.Latest)
                .Where(record => record.Name != name).List<EntityMetadataPart>();
            var isValid = !entities.Any(x => string.Equals(x.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
            var collectionName = settings["CollectionName"];
            var collectionDisplayName = settings["CollectionDisplayName"];
            if (collectionName != collectionName.ToSafeName()) {
                isValid = false;
            }
            var hasDuplicateEntities = (from entity in entities
                let tSetting = entity.EntitySetting
                where tSetting["CollectionName"] == collectionName ||
                      tSetting["CollectionDisplayName"] == collectionDisplayName
                select entity).Any();
            if (hasDuplicateEntities) {
                isValid = false;
            }
            return isValid;
        }

        public EntityMetadataPart GetEntity(int id) {
            return Services.ContentManager.Get<EntityMetadataPart>(id, VersionOptions.Latest);
        }

        public EntityMetadataPart GetEntity(string name) {
            return Services.ContentManager
                .Query<EntityMetadataPart>(VersionOptions.Latest, "EntityMetadata")
                .List().FirstOrDefault(x => x.Name == name);
        }

        public EntityMetadataPart GetDraftEntity(int id) {
            return Services.ContentManager.Get<EntityMetadataPart>(id, VersionOptions.DraftRequired);
        }

        public EntityMetadataPart GetDraftEntity(string name) {
            var entity = GetEntity(name);
            return entity == null
                ? null
                : Services.ContentManager.Get<EntityMetadataPart>(entity.Id, VersionOptions.DraftRequired);
        }

        public string ConstructEntityName(string entityName) {
            var resultName = entityName;
            while (GetEntity(resultName) != null) {
                resultName = VersionName(resultName);
            }
            return resultName;
        }

        public void CreateEntity(EditTypeViewModel sourceModel, IUpdateModel updateModel) {
            var entityDraft = Services.ContentManager.New<EntityMetadataPart>("EntityMetadata");
            entityDraft.DisplayName = sourceModel.DisplayName;
            entityDraft.Name = sourceModel.Name;
            entityDraft.EntitySetting = sourceModel.Settings;

            var field = new FieldMetadataRecord {
                Name = sourceModel.FieldName,
                ContentFieldDefinitionRecord = FetchFieldDefinition(sourceModel.FieldType)
            };
            entityDraft.FieldMetadataRecords.Add(field);
            Services.ContentManager.Create(entityDraft, VersionOptions.Draft);

            var baseFieldSetting = new SettingsDictionary {
                {"DisplayName", sourceModel.FieldLabel},
                {"AddInLayout", bool.TrueString},
                {"EntityName", sourceModel.Name},
                {"Storage", "Part"}
            };
            _entityRecordEditorEvents.FieldSettingsEditorUpdate(sourceModel.FieldType, sourceModel.FieldName, baseFieldSetting, updateModel);
            field.Settings = _settingService.CompileSetting(baseFieldSetting);
            field.EntityMetadataRecord = entityDraft.Record;
        }

        public string DeleteEntity(int id) {
            var entity = GetEntity(id);
            if (entity == null) {
                return "Invalid id";
            }
            foreach (var field in entity.FieldMetadataRecords) {
                _contentDefinitionEditorEvents.FieldDeleted(field.ContentFieldDefinitionRecord.Name, field.Name, _settingService.ParseSetting(field.Settings));
            }
            var hasPublished = entity.HasPublished();

            if (hasPublished) {
                _schemaUpdateService.DropTable(entity.Name);
                _entityEvents.OnDeleting(entity.Name);
                _contentDefinitionService.RemoveType(entity.Name, true);
            }
            Services.ContentManager.Remove(entity.ContentItem);

            return null;
        }

        #endregion

        #region Field Related

        public IEnumerable<FieldMetadataRecord> GetFieldsList(int entityId) {
            return GetEntity(entityId).FieldMetadataRecords;
        }

        public bool CheckFieldCreationValid(EntityMetadataPart entity, string name, string displayName) {
            return !entity.FieldMetadataRecords.Any(
                field => string.Equals(field.Name, name, StringComparison.OrdinalIgnoreCase)
                         || string.Equals(_settingService.ParseSetting(field.Settings)["DisplayName"], displayName, StringComparison.OrdinalIgnoreCase));
        }

        public string ConstructFieldName(string entityName, string displayName) {
            var entity = GetEntity(entityName);
            if (entity == null) {
                throw new ArgumentException("The entity doesn't exist: " + entityName);
            }
            var resultName = displayName;
            while (entity.FieldMetadataRecords.Any(x => String.Equals(resultName, x.Name, StringComparison.OrdinalIgnoreCase))) {
                resultName = VersionName(resultName);
            }
            return resultName;
        }

        public void CreateField(EntityMetadataPart entity, AddFieldViewModel viewModel, IUpdateModel updateModel) {
            var settingsDictionary = new SettingsDictionary();
            settingsDictionary["DisplayName"] = viewModel.DisplayName;
            settingsDictionary["AddInLayout"] = viewModel.AddInLayout.ToString();
            settingsDictionary["EntityName"] = entity.Name;
            var field = new FieldMetadataRecord {
                ContentFieldDefinitionRecord = FetchFieldDefinition(viewModel.FieldTypeName),
                Name = viewModel.Name
            };
            entity.FieldMetadataRecords.Add(field);
            _contentDefinitionEditorEvents.UpdateFieldSettings(viewModel.FieldTypeName, viewModel.Name, settingsDictionary, updateModel);
            field.Settings = _settingService.CompileSetting(settingsDictionary);
            field.EntityMetadataRecord = entity.Record;
        }

        public void UpdateField(FieldMetadataRecord record, string displayName, IUpdateModel updateModel) {
            var settingsDictionary = _settingService.ParseSetting(record.Settings);
            settingsDictionary["DisplayName"] = displayName;
            _contentDefinitionEditorEvents.UpdateFieldSettings(record.ContentFieldDefinitionRecord.Name, record.Name, settingsDictionary, updateModel);
            record.Settings = _settingService.CompileSetting(settingsDictionary);
        }

        public bool DeleteField(string fieldName, string entityName) {
            var entity = GetDraftEntity(entityName);
            if (entity == null) {
                return false;
            }
            var field = entity.FieldMetadataRecords.FirstOrDefault(record => record.Name == fieldName);
            if (field == null) {
                return false;
            }

            _contentDefinitionEditorEvents.FieldDeleted(field.ContentFieldDefinitionRecord.Name, field.Name, _settingService.ParseSetting(field.Settings));
            entity.FieldMetadataRecords.Remove(field);
            return true;
        }

        #endregion

        #region Private Methods

        private ContentFieldDefinitionRecord FetchFieldDefinition(string fieldType) {
            var baseFieldDefinition = _fieldDefinitionRepository.Get(def => def.Name == fieldType);
            if (baseFieldDefinition == null) {
                baseFieldDefinition = new ContentFieldDefinitionRecord {Name = fieldType};
                _fieldDefinitionRepository.Create(baseFieldDefinition);
            }
            return baseFieldDefinition;
        }

        private static string VersionName(string name) {
            int version;
            var nameParts = name.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length > 1 && int.TryParse(nameParts.Last(), out version)) {
                version = version > 0 ? ++version : 2;
                //this could unintentionally chomp something that looks like a version
                name = string.Join("_", nameParts.Take(nameParts.Length - 1));
            }
            else {
                version = 2;
            }
            return string.Format("{0}_{1}", name, version);
        }

        #endregion
    }
}