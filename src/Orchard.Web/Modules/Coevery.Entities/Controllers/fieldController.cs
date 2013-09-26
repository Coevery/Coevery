using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
using Coevery.Entities.Events;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace Coevery.Entities.Controllers {
    public class FieldController : ApiController {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IFieldEvents _fieldEvents;
        private readonly ISchemaUpdateService _schemaUpdateService;

        public FieldController(
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionManager contentDefinitionManager,
            IFieldEvents fieldEvents,
            ISchemaUpdateService schemaUpdateService) {
            _contentDefinitionService = contentDefinitionService;
            _contentDefinitionManager = contentDefinitionManager;
            _fieldEvents = fieldEvents;
            _schemaUpdateService = schemaUpdateService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/metadata/field
        public object Get(string name, int page, int rows) {
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes().Where(c => c.Name == name);

            var query = from type in metadataTypes
                        from field in type.Fields
                        select new {
                            field.Name,
                            field.DisplayName,
                            FieldType = field.FieldDefinition.Name.CamelFriendly(),
                            Type = field.Settings.GetModel<FieldSettings>(field.FieldDefinition.Name + "Settings").IsSystemField
                            ? "System Field" : "User Field",
                            ControllField = string.Empty
                        };

            var totalRecords = query.Count();
            return new {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }

        // DELETE api/metadata/field/name
        public virtual HttpResponseMessage Delete(string name, string parentname) {
            var partDefinition = _contentDefinitionManager.GetPartDefinition(parentname);
            if (partDefinition == null) {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, "Entity is not exist.");
            }
            var fieldDefinition = partDefinition.Fields.FirstOrDefault(x => x.Name == name);
            if (fieldDefinition == null) {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, "Field is not exist.");
            }

            _fieldEvents.OnDeleting(parentname, name);
            _contentDefinitionService.RemoveFieldFromPart(name, parentname);
            _schemaUpdateService.DropColumn(parentname, name);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}