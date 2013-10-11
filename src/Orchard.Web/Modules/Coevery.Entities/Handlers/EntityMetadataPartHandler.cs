using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Coevery.Core.Services;
using Coevery.Entities.Events;
using Coevery.Entities.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Services;
using Orchard.Data;
using Orchard.Logging;
using IContentDefinitionEditorEvents = Coevery.Entities.Settings.IContentDefinitionEditorEvents;

namespace Coevery.Entities.Handlers {
    public class EntityMetadataPartHandler : ContentHandler {
        private readonly IRepository<FieldMetadataRecord> _fieldMetadataRepository;
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ISettingsFormatter _settingsFormatter;
        private readonly IEntityEvents _entityEvents;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IFieldEvents _fieldEvents;
        private readonly IContentDefinitionEditorEvents _contentDefinitionEditorEvents;

        public EntityMetadataPartHandler(
            IRepository<EntityMetadataRecord> entityMetadataRepository,
            IRepository<FieldMetadataRecord> fieldMetadataRepository,
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            ISettingsFormatter settingsFormatter,
            IEntityEvents entityEvents,
            ISchemaUpdateService schemaUpdateService,
            IFieldEvents fieldEvents,
            IContentDefinitionEditorEvents contentDefinitionEditorEvents) {
            _fieldMetadataRepository = fieldMetadataRepository;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            _settingsFormatter = settingsFormatter;
            _entityEvents = entityEvents;
            _schemaUpdateService = schemaUpdateService;
            _fieldEvents = fieldEvents;
            _contentDefinitionEditorEvents = contentDefinitionEditorEvents;

            Filters.Add(StorageFilter.For(entityMetadataRepository));
            OnVersioning<EntityMetadataPart>(OnVersioning);
            OnPublishing<EntityMetadataPart>(OnPublishing);
        }

        private void OnVersioning(VersionContentContext context, EntityMetadataPart existing, EntityMetadataPart building) {
            building.Record.FieldMetadataRecords = new List<FieldMetadataRecord>();
            foreach (var record in existing.Record.FieldMetadataRecords) {
                var newRecord = new FieldMetadataRecord();
                _fieldMetadataRepository.Copy(record, newRecord);
                newRecord.OriginalId = record.Id;
                newRecord.EntityMetadataRecord = building.Record;
                _fieldMetadataRepository.Create(newRecord);
                building.Record.FieldMetadataRecords.Add(newRecord);
            }
        }

        private void OnPublishing(PublishContentContext context, EntityMetadataPart part) {
            if (context.PreviousItemVersionRecord == null) {
                CreateEntity(part);
            }
            else {
                var previousEntity = _contentManager.Get<EntityMetadataPart>(context.Id);
                UpdateEntity(previousEntity, part);
            }
        }

        private void CreateEntity(EntityMetadataPart part) {
            _contentDefinitionManager.AlterTypeDefinition(part.Name, builder => {
                builder.DisplayedAs(part.DisplayName);
                builder.WithPart(part.Name).WithPart("CoeveryCommonPart");
            });

            foreach (var record in part.FieldMetadataRecords) {
                AddField(part.Name, record, false);
            }

            _entityEvents.OnCreated(part.Name);

            _schemaUpdateService.CreateTable(part.Name, context => {
                foreach (var fieldMetadataRecord in part.FieldMetadataRecords) {
                    context.FieldColumn(fieldMetadataRecord.Name,
                        fieldMetadataRecord.ContentFieldDefinitionRecord.Name);
                }
            });
        }

        private void UpdateEntity(EntityMetadataPart previousEntity, EntityMetadataPart entity) {
            _contentDefinitionManager.AlterTypeDefinition(entity.Name, builder =>
                builder.DisplayedAs(entity.DisplayName));

            foreach (var fieldMetadataRecord in previousEntity.FieldMetadataRecords) {
                var exist = entity.FieldMetadataRecords.Any(x => x.OriginalId == fieldMetadataRecord.Id);
                if (!exist) {
                    var record = fieldMetadataRecord;
                    _contentDefinitionManager.AlterPartDefinition(entity.Name,
                        typeBuilder => typeBuilder.RemoveField(record.Name));
                    _schemaUpdateService.DropColumn(entity.Name, fieldMetadataRecord.Name);
                    _fieldEvents.OnDeleting(entity.Name, fieldMetadataRecord.Name);
                }
            }

            var needUpdateFields = new List<FieldMetadataRecord>();
            foreach (var fieldMetadataRecord in entity.FieldMetadataRecords) {
                if (fieldMetadataRecord.OriginalId != 0) {
                    needUpdateFields.Add(fieldMetadataRecord);
                }
                else {
                    AddField(entity.Name, fieldMetadataRecord);
                    _schemaUpdateService.CreateColumn(entity.Name, fieldMetadataRecord.Name, fieldMetadataRecord.ContentFieldDefinitionRecord.Name);
                }
            }

            foreach (var fieldMetadataRecord in needUpdateFields) {
                var record = fieldMetadataRecord;
                var settings = _settingsFormatter.Map(Parse(record.Settings));
                _contentDefinitionManager.AlterPartDefinition(entity.Name, builder =>
                    builder.WithField(record.Name, fieldBuilder => {
                        fieldBuilder.WithDisplayName(settings["DisplayName"]);
                        _contentDefinitionEditorEvents.UpdateFieldSettings(fieldBuilder, settings);
                    }));
                record.Settings = _settingsFormatter.Map(settings).ToString();
            }
            _entityEvents.OnUpdating(entity.Name);
        }

        private void AddField(string entityName, FieldMetadataRecord field, bool needEvent = true) {
            var settings = _settingsFormatter.Map(Parse(field.Settings));

            // add field to part
            _contentDefinitionManager.AlterPartDefinition(entityName, builder => {
                string fieldTypeName = field.ContentFieldDefinitionRecord.Name;
                builder.WithField(field.Name, fieldBuilder =>
                    fieldBuilder.OfType(fieldTypeName).WithSetting("Storage", "Part"));
            });

            // update field settings
            _contentDefinitionManager.AlterPartDefinition(entityName, builder =>
                builder.WithField(field.Name, fieldBuilder =>
                    _contentDefinitionEditorEvents.UpdateFieldSettings(fieldBuilder, settings))
                );

            field.Settings = _settingsFormatter.Map(settings).ToString();

            if (needEvent) {
                _fieldEvents.OnCreated(entityName, field.Name, bool.Parse(settings["AddInLayout"]));
            }
        }

        private XElement Parse(string settings) {
            if (string.IsNullOrEmpty(settings)) {
                return null;
            }

            try {
                return XElement.Parse(settings);
            }
            catch (Exception ex) {
                Logger.Error(ex, "Unable to parse settings xml");
                return null;
            }
        }
    }
}