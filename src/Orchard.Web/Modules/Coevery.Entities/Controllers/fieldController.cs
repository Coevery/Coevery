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
        private readonly IContentMetadataService _contentMetadataService;

        public FieldController(
            IContentMetadataService contentMetadataService) {
            _contentMetadataService = contentMetadataService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/metadata/field
        public object Get(int name, int page, int rows) {
            var metadataTypes = _contentMetadataService.GetFieldsList(name);

            var query = from field in metadataTypes
                        let fieldType = field.ContentFieldDefinitionRecord.Name
                        let setting = _contentMetadataService.ParseSetting(field.Settings)
                        select new {
                            field.Name,
                            field.Id,
                            DisplayName = setting["DisplayName"],
                            FieldType = fieldType.CamelFriendly(),
                            Type = setting.GetModel<FieldSettings>(fieldType+"Settings").IsSystemField
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
        public virtual HttpResponseMessage Delete(int name) {
            return _contentMetadataService.DeleteField(name)
                ?Request.CreateResponse(HttpStatusCode.OK)
                : Request.CreateErrorResponse(HttpStatusCode.BadRequest,"Invalid id!");
        }
    }
}

/*Abandoned Code
var metadataTypes = _contentDefinitionService.GetUserDefinedTypes().Where(c => c.Name == name);
       var fieldName = deleteInfo.FieldName;
            var entityName = deleteInfo.EntityName;
            var partDefinition = _contentDefinitionManager.GetPartDefinition(entityName);
            if (partDefinition == null) {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, "Entity is not exist.");
            }
            var fieldDefinition = partDefinition.Fields.FirstOrDefault(x => x.Name == fieldName);
            if (fieldDefinition == null) {
                return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, "Field is not exist.");
            }

            _fieldEvents.OnDeleting(entityName, fieldName);
            _contentDefinitionService.RemoveFieldFromPart(fieldName, entityName);
            _schemaUpdateService.DropColumn(entityName, fieldName);
            return Request.CreateResponse(HttpStatusCode.OK);
 */