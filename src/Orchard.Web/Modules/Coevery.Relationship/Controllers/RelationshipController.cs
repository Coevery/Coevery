using System.Linq;
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
        private readonly IRepository<ManyToManyRelationshipRecord> _manyToManyRelationshipRepository;
        private readonly IContentDefinitionService _contentDefinitionService;

        public Localizer T { get; set; }

        public RelationshipController(
            IRelationshipService relationshipService,
            IRepository<RelationshipRecord> relationshipRepository,
            IRepository<OneToManyRelationshipRecord> oneToManyRelationshipRepository,
            IRepository<ManyToManyRelationshipRecord> manyToManyRelationshipRepository,
            IContentDefinitionService contentDefinitionService) {
            _relationshipService = relationshipService;
            _relationshipRepository = relationshipRepository;
            _oneToManyRelationshipRepository = oneToManyRelationshipRepository;
            _manyToManyRelationshipRepository = manyToManyRelationshipRepository;
            _contentDefinitionService = contentDefinitionService;
            T = NullLocalizer.Instance;
        }

        public object Get(string entityName) {
            var temp = _relationshipService.GetRelationships(entityName);
            if (temp == null) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The entity doesn't exist!");
            }
            if (temp.Length == 0) {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            return (from record in temp
                    select new {
                        ContentId = record.Id,
                        Name = record.Name,
                        PrimaryEntity = record.PrimaryEntity.Name,
                        RelatedEntity = record.RelatedEntity.Name,
                        Type = ((RelationshipType) record.Type).ToString()
                    }).ToArray();
        }

        public HttpResponseMessage Delete(int relationshipId) {
            var relationship = _relationshipRepository.Get(relationshipId);
            if (relationship == null) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid relationship.");
            }
            if (relationship.Type == (byte) RelationshipType.OneToMany) {
                var record = _oneToManyRelationshipRepository.Get(x => x.Relationship == relationship);
                _relationshipService.DeleteOneToManyRelationship(record);
                _contentDefinitionService.RemoveFieldFromPart(record.LookupField.Name, relationship.RelatedEntity.Name);
            }
            else if (relationship.Type == (byte) RelationshipType.ManyToMany) {
                var record = _manyToManyRelationshipRepository.Get(x => x.Relationship == relationship);
                _relationshipService.DeleteManyToManyRelationship(record);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}