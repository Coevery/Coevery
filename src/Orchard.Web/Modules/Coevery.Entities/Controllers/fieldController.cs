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
        private readonly ILayoutManager _layoutManager;

        public FieldController(
            IContentDefinitionService contentDefinitionService,
            IFieldService fieldService,
            ILayoutManager layoutManager) {
            _contentDefinitionService = contentDefinitionService;
            _fieldService = fieldService;
            _layoutManager = layoutManager;
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
            _layoutManager.DeleteField(parentname, name);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}