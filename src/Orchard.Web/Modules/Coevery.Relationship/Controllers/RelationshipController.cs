using System;
using System.Linq;
using Coevery.Core.Services;
using Coevery.Entities.Events;
using Coevery.Entities.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Relationship.Services;
using Coevery.Relationship.Records;
using Orchard.Data;
using Orchard.Localization;

namespace Coevery.Relationship.Controllers {
    public class RelationshipController : ApiController {
        private readonly IRelationshipService _relationshipService;
        private readonly IRepository<RelationshipRecord> _relationshipRepository;
        private readonly IRepository<OneToManyRelationshipRecord> _oneToManyRelationshipRepository;
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IFieldEvents _fieldEvents;
        private readonly ISchemaUpdateService _schemaUpdateService;

        public Localizer T { get; set; }

        public RelationshipController(
            IRelationshipService relationshipService,
            IRepository<RelationshipRecord> relationshipRepository,
            IRepository<OneToManyRelationshipRecord> oneToManyRelationshipRepository,
            IContentDefinitionService contentDefinitionService,
            IFieldEvents fieldEvents,
            ISchemaUpdateService schemaUpdateService) {
            _relationshipService = relationshipService;
            _relationshipRepository = relationshipRepository;
            _oneToManyRelationshipRepository = oneToManyRelationshipRepository;
            _contentDefinitionService = contentDefinitionService;
            _fieldEvents = fieldEvents;
            _schemaUpdateService = schemaUpdateService;
            T = NullLocalizer.Instance;
        }

        public object Get(string entityName, int page, int rows) {
            var temp = _relationshipService.GetRelationships(entityName);
            if (temp == null) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The entity doesn't exist!");
            }
            if (temp.Length == 0) {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            var query = from record in temp
                        select new {
                            ContentId = record.Id,
                            Name = record.Name,
                            PrimaryEntity = record.PrimaryEntity.Name,
                            RelatedEntity = record.RelatedEntity.Name,
                            Type = ((RelationshipType) record.Type).ToString()
                        };
            var totalRecords = query.Count();
            return new {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }

        public HttpResponseMessage Delete(int relationshipId) {
            var relationship = _relationshipRepository.Get(relationshipId);
            if (relationship == null) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid relationship.");
            }

            if (relationship.Type == (byte)RelationshipType.OneToMany) {
                var record = _oneToManyRelationshipRepository.Get(x => x.Relationship == relationship);
                string entityName = relationship.RelatedEntity.Name;
                string fieldName = record.LookupField.Name;
                _fieldEvents.OnDeleting(entityName, fieldName);
                _contentDefinitionService.RemoveFieldFromPart(fieldName, entityName);
                _schemaUpdateService.DropColumn(entityName, fieldName);
            } else if (relationship.Type == (byte)RelationshipType.ManyToMany) {
                _relationshipService.DeleteRelationship(relationship);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}