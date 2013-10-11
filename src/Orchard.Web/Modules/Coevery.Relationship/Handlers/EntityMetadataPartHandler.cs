using System.Linq;
using Coevery.Entities.Models;
using Coevery.Relationship.Records;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Coevery.Relationship.Handlers {
    public class EntityMetadataPartHandler : ContentHandler {
        private readonly IRepository<RelationshipRecord> _relationshipRepository;
        private readonly IRepository<OneToManyRelationshipRecord> _oneToManyRelationshipRepository;
        private readonly IRepository<ManyToManyRelationshipRecord> _manyToManyRelationshipRepository;

        public EntityMetadataPartHandler(
            IRepository<RelationshipRecord> relationshipRepository,
            IRepository<OneToManyRelationshipRecord> oneToManyRelationshipRepository,
            IRepository<ManyToManyRelationshipRecord> manyToManyRelationshipRepository) {
            _relationshipRepository = relationshipRepository;
            _oneToManyRelationshipRepository = oneToManyRelationshipRepository;
            _manyToManyRelationshipRepository = manyToManyRelationshipRepository;

            OnVersioned<EntityMetadataPart>(OnVersioned);
        }

        private void OnVersioned(VersionContentContext context, EntityMetadataPart existing, EntityMetadataPart building) {
            CopyRelationshipRecords(_oneToManyRelationshipRepository, existing.Record, building.Record);
            CopyRelationshipRecords(_manyToManyRelationshipRepository, existing.Record, building.Record);
        }

        private void CopyRelationshipRecords<T>(IRepository<T> repository, EntityMetadataRecord existing, EntityMetadataRecord building) where T : IRelationshipRecord, new() {
            var records = repository.Table
                .Where(x => (x.Relationship.PrimaryEntity == existing && x.Relationship.RelatedEntity == existing)
                    || (x.Relationship.PrimaryEntity == existing && x.Relationship.RelatedEntity.ContentItemVersionRecord.Latest)
                    || (x.Relationship.RelatedEntity == existing && x.Relationship.PrimaryEntity.ContentItemVersionRecord.Latest))
                .ToList();

            foreach (var record in records) {
                var relationshipRecord = GetNewRelationshipRecord(record.Relationship, existing, building);
                var newRecord = new T();
                repository.Copy(record, newRecord);
                newRecord.Relationship = relationshipRecord;
                repository.Create(newRecord);
            }
        }

        private RelationshipRecord GetNewRelationshipRecord(RelationshipRecord record, EntityMetadataRecord existing, EntityMetadataRecord building) {
            var newRecord = new RelationshipRecord();
            _relationshipRepository.Copy(record, newRecord);
            if (newRecord.PrimaryEntity == existing) {
                newRecord.PrimaryEntity = building;
            }
            if (newRecord.RelatedEntity == existing) {
                newRecord.RelatedEntity = building;
            }
            _relationshipRepository.Create(newRecord);
            return newRecord;
        }
    }
}