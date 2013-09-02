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

        public void OnDeleting(FieldEventsContext context) {
            var partDefinition = _contentDefinitionManager.GetPartDefinition(context.EtityName);
            if (partDefinition == null) {
                return;
            }
            var fieldDefinition = partDefinition.Fields.FirstOrDefault(x => x.Name == context.FieldName);
            if (fieldDefinition == null
                || fieldDefinition.FieldDefinition.Name != "ReferenceField") {
                return;
            }

            var oneToManyRecord = _oneToManyRepository
                .Get(x => x.Relationship.RelatedEntity.Name == context.EtityName
                          && x.LookupField.Name == context.FieldName);
            if (oneToManyRecord != null) {
                _relationshipService.DeleteOneToManyRelationship(oneToManyRecord);
            }
        }
    }
}