using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Coevery.Entities.Models;
using Coevery.Entities.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.MetaData.Services;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using IContentDefinitionEditorEvents = Coevery.Entities.Settings.IContentDefinitionEditorEvents;

namespace Coevery.Entities.Services {
    public interface IContentMetadataService : IDependency {
        bool CreateEntity(EditTypeViewModel sourceModel);
        IEnumerable<EntityMetadataPart> GetRawEntities();
        EntityMetadataPart GetEntity(int id);
        EntityMetadataPart GetEntity(string name);
        EntityMetadataPart GetDraftEntity(string name);
        bool CheckEntityCreationValid(string name, string displayName);
        bool CheckEntityPublished(string name);
        bool CheckEntityDisplayValid(string name, string displayName);
        string ConstructEntityName(string entityName);
        string TryDeleteEntity(int id);

        IEnumerable<FieldMetadataRecord> GetFieldsList(int entityId);
        SettingsDictionary ParseSetting(string setting);
        string ConstructFieldName(string entityName, string displayName);
        bool CheckFieldCreationValid(EntityMetadataPart entity, string name, string displayName);
        void CreateField(EntityMetadataPart entity, AddFieldViewModel viewModel, IUpdateModel updateModel);
        FieldWithEntityInfoModel TryDeleteField(int id);
        void UpdateField(FieldMetadataRecord record, string displayName, IUpdateModel updateModel);
    }

    public class ContentMetadataService : IContentMetadataService {
        private readonly ISettingsFormatter _settingsFormatter;
        private readonly IRepository<ContentFieldDefinitionRecord> _fieldDefinitionRepository;
        private readonly IRepository<FieldMetadataRecord> _fieldMetadataRepository;
        private readonly IContentDefinitionEditorEvents _contentDefinitionEditorEvents;

        public ContentMetadataService(
            IOrchardServices services,
            ISettingsFormatter settingsFormatter,
            IRepository<ContentFieldDefinitionRecord> fieldDefinitionRepository,
            IRepository<FieldMetadataRecord> fieldMetadataRepository,
            IContentDefinitionEditorEvents contentDefinitionEditorEvents) {
            _settingsFormatter = settingsFormatter;
            _fieldDefinitionRepository = fieldDefinitionRepository;
            _fieldMetadataRepository = fieldMetadataRepository;
            _contentDefinitionEditorEvents = contentDefinitionEditorEvents;
            Services = services;
        }

        public IOrchardServices Services { get; private set; }

        #region Entity Related

        public IEnumerable<EntityMetadataPart> GetRawEntities() {
            return Services.ContentManager.Query<EntityMetadataPart, EntityMetadataRecord>()
                .ForVersion(VersionOptions.Latest).List();
        }

        public bool CheckEntityCreationValid(string name, string displayName) {
            return !Services.ContentManager
                .Query<EntityMetadataPart>(VersionOptions.Latest, "EntityMetadata").List()
                .Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)
                          || string.Equals(x.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
        }

        public bool CheckEntityPublished(string name) {
            return GetEntity(name).ContentItem.VersionRecord.Published;
        }

        public bool CheckEntityDisplayValid(string name, string displayName) {
            return !Services.ContentManager
                .Query<EntityMetadataPart>(VersionOptions.Latest, "EntityMetadata").List()
                .Any(x => !string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)
                          && string.Equals(x.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
        }

        public EntityMetadataPart GetEntity(int id) {
            return Services.ContentManager.Get<EntityMetadataPart>(id, VersionOptions.Latest);
        }

        public EntityMetadataPart GetEntity(string name) {
            return Services.ContentManager
                .Query<EntityMetadataPart>(VersionOptions.Latest, "EntityMetadata")
                .List().FirstOrDefault(x => x.Name == name);
        }

        public EntityMetadataPart GetDraftEntity(string name) {
            var entity = GetEntity(name);
            return entity == null ?
                null :
                Services.ContentManager.Get<EntityMetadataPart>(entity.Id, VersionOptions.DraftRequired);
        }

        public string ConstructEntityName(string entityName) {
            var resultName = entityName;
            while (GetEntity(resultName) != null) {
                resultName = VersionName(resultName);
            }
            return resultName;
        }

        public bool CreateEntity(EditTypeViewModel sourceModel) {
            var entityDraft = Services.ContentManager.New<EntityMetadataPart>("EntityMetadata");
            var baseFieldSetting = new SettingsDictionary {
                {"DisplayName", sourceModel.FieldLabel},
                {"Storage", "Part"},
                {"CoeveryTextFieldSettings.IsDispalyField", bool.TrueString},
                {"CoeveryTextFieldSettings.Required", bool.TrueString},
                {"CoeveryTextFieldSettings.ReadOnly", bool.TrueString},
                {"CoeveryTextFieldSettings.AlwaysInLayout", bool.TrueString},
                {"CoeveryTextFieldSettings.IsSystemField", bool.TrueString},
                {"CoeveryTextFieldSettings.IsAudit", bool.FalseString}
            };
            entityDraft.DisplayName = sourceModel.DisplayName;
            entityDraft.Name = sourceModel.Name;
            //entityDraft.Settings = sourceModel.Settings;

            entityDraft.FieldMetadataRecords.Add(new FieldMetadataRecord {
                Name = sourceModel.FieldName,
                ContentFieldDefinitionRecord = FetchFieldDefinition("CoeveryTextField"),
                Settings = CompileSetting(baseFieldSetting)
            });
            Services.ContentManager.Create(entityDraft, VersionOptions.Draft);
            return true;
        }

        public string TryDeleteEntity(int id) {
            var entity = GetEntity(id);
            if (entity == null) {
                throw new NullReferenceException("Invalid id");
            }
            foreach (var field in entity.FieldMetadataRecords) {
                if (field.ContentFieldDefinitionRecord.Name == "OptionSetField") {
                    _contentDefinitionEditorEvents.CustomDeleteAction(field.ContentFieldDefinitionRecord.Name, field.Name, ParseSetting(field.Settings));
                }
            }
            entity.FieldMetadataRecords.Clear();
            Services.ContentManager.Remove(entity.ContentItem);
            if (!entity.ContentItem.VersionRecord.Published) {
                return null;
            }
            return entity.Name;
        }

        #endregion

        #region Field Related

        public IEnumerable<FieldMetadataRecord> GetFieldsList(int entityId) {
            return GetEntity(entityId).FieldMetadataRecords;
        }

        public SettingsDictionary ParseSetting(string setting) {
            return string.IsNullOrWhiteSpace(setting) ? null
                : _settingsFormatter.Map(XElement.Parse(setting));
        }

        private string CompileSetting(SettingsDictionary settings) {
            return settings == null
                ? null
                : _settingsFormatter.Map(settings).ToString();
        }

        public bool CheckFieldCreationValid(EntityMetadataPart entity, string name, string displayName) {
            return !entity.FieldMetadataRecords.Any(
                field => string.Equals(field.Name, name, StringComparison.OrdinalIgnoreCase)
                         || string.Equals(ParseSetting(field.Settings)["DisplayName"], displayName, StringComparison.OrdinalIgnoreCase));
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
            _contentDefinitionEditorEvents.UpdateFieldSettings(viewModel.FieldTypeName, settingsDictionary, updateModel);
            entity.FieldMetadataRecords.Add(new FieldMetadataRecord {
                ContentFieldDefinitionRecord = FetchFieldDefinition(viewModel.FieldTypeName),
                Name = viewModel.Name,
                Settings = CompileSetting(settingsDictionary)
            });
        }

        public void UpdateField(FieldMetadataRecord record, string displayName, IUpdateModel updateModel) {
            var settingsDictionary = ParseSetting(record.Settings);
            settingsDictionary["DisplayName"] = displayName;
            _contentDefinitionEditorEvents.UpdateFieldSettings(record.ContentFieldDefinitionRecord.Name, record.Name, settingsDictionary, updateModel);
            record.Settings = CompileSetting(settingsDictionary);
        }

        public FieldWithEntityInfoModel TryDeleteField(int id) {
            var field = _fieldMetadataRepository.Get(id);
            var entity = Services.ContentManager.Get(field.EntityMetadataRecord.Id, VersionOptions.Latest).As<EntityMetadataPart>();
            if (entity == null) {
                throw new NullReferenceException("Invalid id");
            }
            if (field.ContentFieldDefinitionRecord.Name == "OptionSetField") {
                _contentDefinitionEditorEvents.CustomDeleteAction(field.ContentFieldDefinitionRecord.Name, field.Name, ParseSetting(field.Settings));
            }
            entity.FieldMetadataRecords.Remove(field);
            if (!entity.ContentItem.VersionRecord.Published) {
                return null;
            }
            return new FieldWithEntityInfoModel {
                EntityId = entity.Id,
                EntityName = entity.Name,
                FieldId = field.Id,
                FieldName = field.Name
            };
        }

        #region Field Private Methods

        private ContentFieldDefinitionRecord FetchFieldDefinition(string fieldType) {
            var baseFieldDefinition = _fieldDefinitionRepository.Get(def => def.Name == fieldType);
            if (baseFieldDefinition == null) {
                baseFieldDefinition = new ContentFieldDefinitionRecord {Name = fieldType};
                _fieldDefinitionRepository.Create(baseFieldDefinition);
            }
            return baseFieldDefinition;
        }

        #endregion

        #endregion

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
    }
}