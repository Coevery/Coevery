using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Entities.Services;
using Coevery.ContentManagement;
using Coevery.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coevery.Entities.Controllers {
    public class EntityController : ApiController {
        private readonly IContentMetadataService _contentMetadataService;
        private readonly IContentDefinitionService _contentDefinitionService;
        public EntityController(
            IContentMetadataService contentMetadataService,
            IContentDefinitionService contentDefinitionService){
            _contentMetadataService = contentMetadataService;
            _contentDefinitionService = contentDefinitionService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        //QUERY api/Entities/Entity
        public HttpResponseMessage Get()
        {
            var entitiesJ = new List<JObject>();
            try
            {
                var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
                var entityList = metadataTypes.Select(item => item.Name).ToList();
                foreach (var entity in entityList)
                {
                    var entityJ = new JObject();
                    entityJ["name"] = entity;
                    entitiesJ.Add(entityJ);
                }
            }
            catch (Exception)
            {
            }
            var json = JsonConvert.SerializeObject(entitiesJ);
            return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
        }

        //GET api/Entities/Entity
        public object Get(int rows, int page) {
            var metadataTypes = _contentMetadataService.GetRawEntities();

            var query = from type in metadataTypes
                        select new {
                            Id = type.Id,
                            Name = type.Name, 
                            DisplayName = type.DisplayName,
                            Modified = !type.IsPublished(),
                            HasPublished = type.HasPublished()
                        };

            var totalRecords = query.Count();

            return new {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }

        // DELETE api/Entities/Entity/:entityName
        public virtual HttpResponseMessage Delete(int name) {
            var errormessage = _contentMetadataService.DeleteEntity(name);
            if (string.IsNullOrWhiteSpace(errormessage)) {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.ExpectationFailed,errormessage);
        }
    }
}