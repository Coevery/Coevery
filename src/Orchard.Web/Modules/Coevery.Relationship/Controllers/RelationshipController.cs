using System;
using System.Linq;
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

        public Localizer T { get; set; }

        public RelationshipController(
            IRelationshipService relationshipService,
            IRepository<RelationshipRecord> relationshipRepository) {
            _relationshipService = relationshipService;
            _relationshipRepository = relationshipRepository;
            T = NullLocalizer.Instance;
        }

        public object GetRelationNameValidation(string relationName) {
            return new { ErrorMessage = _relationshipService.CheckRelationName(relationName)};
        }

        public object Get(string entityName, int page, int rows) {
            var temp = _relationshipService.GetRelationships(entityName);
            //if (temp == null) {
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The entity doesn't exist!");
            //}
            if (temp == null || temp.Length == 0) {
                return new {
                    total = 0,
                    page = page,
                    records = 0,
                    rows = string.Empty
                };
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
                total = Convert.ToInt32(Math.Ceiling((double) totalRecords / rows)),
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

            _relationshipService.DeleteRelationship(relationship);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}