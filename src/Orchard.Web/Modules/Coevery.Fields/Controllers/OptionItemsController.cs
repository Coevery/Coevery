using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Fields.Records;
using Coevery.Fields.Services;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Orchard.Localization;

namespace Coevery.Fields.Controllers {
    public class OptionItemsController : ApiController {
        private readonly IOptionItemService _optionItemService;

        public OptionItemsController(IOptionItemService optionItemService) {
            _optionItemService = optionItemService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/metadata/OptionItems
        public object Get(string entityName, string fieldName) {
            return _optionItemService.GetItemsForField(entityName, fieldName) ?? Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // PUT api/metadata/OptionItems?Id=1
        public HttpResponseMessage Put(int id, OptionItemRecord optionItem) {
            return _optionItemService.EditItem(id, optionItem)?
                Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // POST api/metadata/OptionItems
        public HttpResponseMessage PostContent(string entityName, string fieldName, OptionItemRecord optionItem) {
            return _optionItemService.CreateItem(entityName, fieldName, optionItem) ?
                Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // DELETE api/metadata/OptionItems?Id=1
        public HttpResponseMessage Delete(int id) {
            return _optionItemService.DeleteItem(id) ?
                Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}