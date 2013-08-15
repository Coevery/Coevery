using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
using Coevery.Entities.Services;
using Coevery.FormDesigner.Services;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace Coevery.Entities.Controllers {
    public class FieldController : ApiController {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IFieldService _fieldService;
        public FieldController(IContentDefinitionService contentDefinitionService,
            IContentDefinitionManager contentDefinitionManager, 
            ISchemaUpdateService schemaUpdateService,
            IFieldService fieldService) {
            _contentDefinitionService = contentDefinitionService;
            _fieldService = fieldService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/metadata/field
        public object Get(string name) {
            var type = _contentDefinitionService.GetType(name);
            return type.Fields.Select(f => new { f.DisplayName, Name = f.FieldDefinition.Name.CamelFriendly() }).ToList();
        }

        // DELETE api/metadata/field/name
        public virtual HttpResponseMessage Delete(string name, string parentname) {
            _fieldService.Delete(name,parentname);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}