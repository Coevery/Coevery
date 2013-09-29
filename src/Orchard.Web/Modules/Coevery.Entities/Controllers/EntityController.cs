using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
using Coevery.Core.Models;
using Coevery.Entities.Events;
using Coevery.Entities.Models;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace Coevery.Entities.Controllers {
    public class EntityController : ApiController {
        private readonly IContentMetadataService _contentMetadataService;

        public EntityController(
            IContentMetadataService contentMetadataService) {
            _contentMetadataService = contentMetadataService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

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