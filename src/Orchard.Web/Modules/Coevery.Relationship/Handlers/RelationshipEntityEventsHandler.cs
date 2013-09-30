using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Services;
using Coevery.Entities.Events;
using Coevery.Entities.Services;
using Coevery.Relationship.Records;
using Coevery.Relationship.Services;
using Orchard.Data;

namespace Coevery.Relationship.Handlers {
    public class RelationshipEntityEventsHandler : IEntityEvents {
        private readonly IRelationshipService _relationshipService;
        private readonly IRepository<OneToManyRelationshipRecord> _oneToManyRelationshipRepository;
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IFieldEvents _fieldEvents;
        private readonly ISchemaUpdateService _schemaUpdateService;

        public RelationshipEntityEventsHandler(
            IRelationshipService relationshipService,
            IRepository<OneToManyRelationshipRecord> oneToManyRelationshipRepository,
            IContentDefinitionService contentDefinitionService, IFieldEvents fieldEvents,
            ISchemaUpdateService schemaUpdateService) {
            _relationshipService = relationshipService;
            _oneToManyRelationshipRepository = oneToManyRelationshipRepository;
            _contentDefinitionService = contentDefinitionService;
            _fieldEvents = fieldEvents;
            _schemaUpdateService = schemaUpdateService;
        }

        public void OnCreated(string entityName) {}
        public void OnUpdating(string entityName) {}

        public void OnDeleting(string entityName) {
            var relationships = _relationshipService.GetRelationships(entityName);
            foreach (var relationship in relationships) {
                if (relationship.Type == (byte) RelationshipType.OneToMany
                    && relationship.RelatedEntity.Name != entityName) {
                    var record = _oneToManyRelationshipRepository.Get(x => x.Relationship == relationship);
                    string relatedEntityName = relationship.RelatedEntity.Name;
                    string fieldName = record.LookupField.Name;
                    _fieldEvents.OnDeleting(relatedEntityName, fieldName);
                    _contentDefinitionService.RemoveFieldFromPart(fieldName, relatedEntityName);
                    _schemaUpdateService.DropColumn(relatedEntityName, fieldName);
                }
                else {
                    _relationshipService.DeleteRelationship(relationship);
                }
            }
        }
    }
}