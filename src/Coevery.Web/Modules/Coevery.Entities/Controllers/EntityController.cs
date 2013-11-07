using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Common.Extensions;
using Coevery.Entities.Services;
using Coevery.ContentManagement;
using Coevery.Entities.ViewModels;
using Coevery.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coevery.Entities.Controllers {
    public class EntityController : ApiController {
        private readonly IContentMetadataService _contentMetadataService;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;
        public EntityController(
            IContentMetadataService contentMetadataService,
            IContentDefinitionExtension contentDefinitionService)
        {
            _contentMetadataService = contentMetadataService;
            _contentDefinitionExtension = contentDefinitionService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        //QUERY api/Entities/Entity
        public HttpResponseMessage Get()
        {
            var entities = new List<JObject>();
            var usertypelist = _contentDefinitionExtension.ListUserDefinedTypeDefinitions();
            if(usertypelist!=null)
            {
                var metadataTypes = usertypelist.Select(ctd => new EditTypeViewModel(ctd)).OrderBy(m => m.DisplayName);
                var entityList = metadataTypes.Select(item => item.Name).ToList();
                foreach (var entity in entityList)
                {
                    var entityitem = new JObject();
                    entityitem["name"] = entity;
                    entities.Add(entityitem);
                }
            }
            var json = JsonConvert.SerializeObject(entities);
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