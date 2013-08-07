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

        //// PUT api/metadata/OptionItems?Id=1
        //public virtual HttpResponseMessage Put(int id, OptionItemRecord optionItem) {
        //    var optionItemRecord = _optionItemRepository.Table.SingleOrDefault(x => x.Id == id);
        //    if (optionItemRecord == null) {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }
        //    ResetDefault(optionItemRecord.ContentPartFieldDefinitionRecord);
        //    optionItemRecord.Value = optionItem.Value;
        //    optionItemRecord.IsDefault = optionItem.IsDefault;
        //    _optionItemRepository.Update(optionItemRecord);

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

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

        //private void ResetDefault(ContentPartFieldDefinitionRecord fieldDefinitionRecord) {
        //    var defaultItem = _optionItemRepository.Table.SingleOrDefault(x => x.ContentPartFieldDefinitionRecord == fieldDefinitionRecord && x.IsDefault);
        //    if (defaultItem != null) {
        //        defaultItem.IsDefault = false;
        //    }
        //}
        //        _optionItemRepository.Update(defaultItem);
    }
}