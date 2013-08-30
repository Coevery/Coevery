using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Entities.Services;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace Coevery.Entities.Controllers {
    public class FieldController : ApiController {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public FieldController(
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionManager contentDefinitionManager) {
            _contentDefinitionService = contentDefinitionService;
            _contentDefinitionManager = contentDefinitionManager;
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

            _contentDefinitionService.RemoveFieldFromPart(name, parentname);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}