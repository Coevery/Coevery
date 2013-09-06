using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Fields.Services;
using Orchard.Localization;

namespace Coevery.Fields.Controllers {
    public class FieldDependencyController : ApiController {
        private readonly IFieldDependencyService _fieldDependencyService;

        public FieldDependencyController(
            IFieldDependencyService fieldDependencyService) {
            _fieldDependencyService = fieldDependencyService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/fields/FieldDependency?entityName=lead
        public object Get(string entityName) {
            return _fieldDependencyService.Get(entityName) ?? Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // PUT api/fields/FieldDependency?Id=1
        public virtual HttpResponseMessage Put(int id, string newValue) {
            return _fieldDependencyService.Edit(id, newValue) ?
                       Request.CreateResponse(HttpStatusCode.OK) :
                       Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // POST api/fields/FieldDependency
        public virtual HttpResponseMessage Post(string entityName, string controlFieldName, string dependentFieldName, DependencyValuePair[] value) {
            return _fieldDependencyService.Create(entityName, controlFieldName, dependentFieldName, value) ?
                Request.CreateResponse(HttpStatusCode.Created)
                : Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // DELETE api/fields/FieldDependency?Id=1
        public HttpResponseMessage Delete(int id) {
            return _fieldDependencyService.Delete(id) ?
                       Request.CreateResponse(HttpStatusCode.OK)
                       : Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}