using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Relationship.Services;
using Coevery.Relationship.Records;
using Orchard.Localization;

namespace Coevery.Relationship.Controllers
{
    public class RelationshipController : ApiController
    {
        private readonly IRelationshipService _relationshipService;
        public Localizer T { get; set; }

        public RelationshipController(
            IRelationshipService relationshipService) {
            _relationshipService = relationshipService;
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
            if (relationshipId <= 0) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Id");
            }

            var errorMessage = _relationshipService.DeleteRelationship(relationshipId);
            return string.IsNullOrWhiteSpace(errorMessage)
                ?Request.CreateResponse(HttpStatusCode.OK)
                :Request.CreateErrorResponse(HttpStatusCode.BadRequest,errorMessage);
        }
    }
}
