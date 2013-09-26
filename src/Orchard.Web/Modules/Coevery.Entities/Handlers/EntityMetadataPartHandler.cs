using System;
using System.Collections.Generic;
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

namespace Coevery.Entities.Handlers {
    public class EntityMetadataPartHandler : ContentHandler {
        private readonly IRepository<EntityMetadataRecord> _entityMetadataRepository;
        private readonly IRepository<FieldMetadataRecord> _fieldMetadataRepository;
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionService;
        private readonly ISettingsFormatter _settingsFormatter;
        private readonly IEntityEvents _entityEvents;
        private readonly ISchemaUpdateService _schemaUpdateService;

        public EntityMetadataPartHandler(
            IRepository<EntityMetadataRecord> entityMetadataRepository,
            IRepository<FieldMetadataRecord> fieldMetadataRepository,
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionService,
            ISettingsFormatter settingsFormatter,
            IEntityEvents entityEvents,
            ISchemaUpdateService schemaUpdateService) {
            _entityMetadataRepository = entityMetadataRepository;
            _fieldMetadataRepository = fieldMetadataRepository;
            _contentManager = contentManager;
            _contentDefinitionService = contentDefinitionService;
            _settingsFormatter = settingsFormatter;
            _entityEvents = entityEvents;
            _schemaUpdateService = schemaUpdateService;

            Filters.Add(StorageFilter.For(entityMetadataRepository));
        }

        protected override void Versioning(VersionContentContext context) {
            var building = context.BuildingContentItem.As<EntityMetadataPart>();
            var existing = context.ExistingContentItem.As<EntityMetadataPart>();

            building.Record.FieldMetadataRecords = new List<FieldMetadataRecord>();
            foreach (var record in existing.Record.FieldMetadataRecords) {
                var newRecord = new FieldMetadataRecord();
                _fieldMetadataRepository.Copy(record, newRecord);
                newRecord.OriginalId = record.Id;
                _fieldMetadataRepository.Create(newRecord);
                building.Record.FieldMetadataRecords.Add(newRecord);
            }
        }

        protected override void Publishing(PublishContentContext context) {
            var previous = context.PreviousItemVersionRecord;
            var publishing = context.PublishingItemVersionRecord;

            var entity = context.ContentItem.As<EntityMetadataPart>();
            if (context.PreviousItemVersionRecord == null) {
                _contentDefinitionService.AlterPartDefinition(entity.Name, builder => {
                    foreach (var fieldMetadataRecord in entity.FieldMetadataRecords) {
                        var settings = _settingsFormatter.Map(Parse(fieldMetadataRecord.Settings));
                        string fieldTypeName = fieldMetadataRecord.ContentFieldDefinitionRecord.Name;

                        builder.WithField(fieldMetadataRecord.Name, fieldBuilder => {
                            fieldBuilder.OfType(fieldTypeName);
                            foreach (var setting in settings) {
                                fieldBuilder.WithSetting(setting.Key, setting.Value);
                            }
                        });
                    }
                });

                _contentDefinitionService.AlterTypeDefinition(entity.Name, builder => {
                    builder.DisplayedAs(entity.DisplayName);
                    builder.WithPart(entity.Name).WithPart("CoeveryCommonPart");
                });

                _entityEvents.OnCreated(entity.Name);

                _schemaUpdateService.CreateTable(entity.Name, cyx => {
                    foreach (var fieldMetadataRecord in entity.FieldMetadataRecords) {
                        cyx.FieldColumn(fieldMetadataRecord.Name,
                            fieldMetadataRecord.ContentFieldDefinitionRecord.Name);
                    }
                });
            }
            else {
                var previousEntity = _contentManager.Get<EntityMetadataPart>(context.Id);
                _contentDefinitionService.AlterTypeDefinition(entity.Name, builder => 
                    builder.DisplayedAs(entity.DisplayName));


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