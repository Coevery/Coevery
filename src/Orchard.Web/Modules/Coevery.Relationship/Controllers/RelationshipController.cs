using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            if (temp.Length == 0) {
                return null;
            }
            return JsonConvert.SerializeObject((from record in temp
                                                   select new {
                                                       ContentId = record.Id,
                                                       Name = record.Name,
                                                       PrimaryEntity = record.PrimaryEntity.Name,
                                                       RelatedEntity = record.RelatedEntity.Name,
                                                       Type = ((RelationshipType)record.Type).ToString()
                                                   }).ToArray());
        }

    }
}
