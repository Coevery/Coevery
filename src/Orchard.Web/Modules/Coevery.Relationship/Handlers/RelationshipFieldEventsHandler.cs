using System.Linq;
using Coevery.Entities.Events;
using Coevery.Relationship.Records;
using Coevery.Relationship.Services;
using Orchard.ContentManagement.MetaData;
using Orchard.Data;

namespace Coevery.Relationship.Handlers {
    public class RelationshipFieldEventsHandler : IFieldEvents {
        private readonly IRelationshipService _relationshipService;
        private readonly IRepository<OneToManyRelationshipRecord> _oneToManyRepository;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public RelationshipFieldEventsHandler(
            IRepository<OneToManyRelationshipRecord> oneToManyRepository,
            IRelationshipService relationshipService,
            IContentDefinitionManager contentDefinitionManager) {
            _oneToManyRepository = oneToManyRepository;
            _relationshipService = relationshipService;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public void OnCreated(string etityName, string fieldName, bool isInLayout) {}

        public void OnDeleting(string etityName, string fieldName) {
            var partDefinition = _contentDefinitionManager.GetPartDefinition(etityName);
            if (partDefinition == null) {
                return;
            }
            var fieldDefinition = partDefinition.Fields.FirstOrDefault(x => x.Name == fieldName);
            if (fieldDefinition == null
                || fieldDefinition.FieldDefinition.Name != "ReferenceField") {
                return;
            }

            var oneToManyRecord = _oneToManyRepository
                .Get(x => x.Relationship.RelatedEntity.Name == etityName
                          && x.LookupField.Name == fieldName);
            if (oneToManyRecord != null) {
                _relationshipService.DeleteRelationship(oneToManyRecord.Relationship);
            }
        }
    }
}