using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
using Coevery.Entities.Events;
using Coevery.Entities.Services;
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
        public object Get(string name) {
            var type = _contentDefinitionService.GetType(name);
            return type.Fields.Select(f => new {f.DisplayName, Name = f.FieldDefinition.Name.CamelFriendly()}).ToList();
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