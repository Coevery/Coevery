using System.Collections.Generic;
using Coevery.Entities.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Coevery.Entities.Handlers {
    public class EntityMetadataPartHandler : ContentHandler {
        private readonly IRepository<FieldMetadataRecord> _fieldMetadataRepository;

        public EntityMetadataPartHandler(
            IRepository<EntityMetadataRecord> repository,
            IRepository<FieldMetadataRecord> fieldMetadataRepository) {
            _fieldMetadataRepository = fieldMetadataRepository;

            Filters.Add(StorageFilter.For(repository));

            OnPublished<EntityMetadataPart>((context, part) => { });
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
    }
}