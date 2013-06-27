using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Fields.Records;
using Newtonsoft.Json.Linq;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Orchard.Localization;

namespace Coevery.Fields.Controllers {
    public class FieldDependencyController : ApiController {
        private readonly IRepository<FieldDependencyRecord> _fieldDependencyRepository;
        private readonly IRepository<ContentPartDefinitionRecord> _partDefinitionRepository;

        public FieldDependencyController(
            IRepository<FieldDependencyRecord> fieldDependencyRepository,
            IRepository<ContentPartDefinitionRecord> partDefinitionRepository) {
            _fieldDependencyRepository = fieldDependencyRepository;
            _partDefinitionRepository = partDefinitionRepository;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/fields/FieldDependency?entityName=lead
        public object Get(string entityName) {
            var partDefinition = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (partDefinition == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var dependencies = _fieldDependencyRepository.Table
                .Where(x => x.Entity == partDefinition)
                .Select(x => new {
                    x.Id,
                    ControlFieldName = x.ControlField.Name,
                    DependentFieldName = x.DependentField.Name
                });
            return dependencies;
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

        // POST api/fields/OptionItems
        public virtual HttpResponseMessage Post(string entityName, string controlFieldName, string dependentFieldName, JObject value) {
            var partDefinition = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (partDefinition == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var controlField = partDefinition.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == controlFieldName);
            var dependentField = partDefinition.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == dependentFieldName);
            if (controlField == null || dependentField == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var dependencyValue = value.Value<string>("Value");
            if (dependencyValue == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var dependencyRecord = _fieldDependencyRepository.Table.SingleOrDefault(x => x.Entity == partDefinition && x.ControlField == controlField && x.DependentField == dependentField);
            if (dependencyRecord != null) {
                return Request.CreateResponse(HttpStatusCode.NonAuthoritativeInformation);
            }
            dependencyRecord = new FieldDependencyRecord {
                Entity = partDefinition,
                ControlField = controlField,
                DependentField = dependentField,
                Value = dependencyValue
            };
            _fieldDependencyRepository.Create(dependencyRecord);

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        // DELETE api/metadata/FieldDependency?Id=1
        public HttpResponseMessage Delete(int id) {
            var optionItem = _fieldDependencyRepository.Table.SingleOrDefault(x => x.Id == id);
            if (optionItem == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            _fieldDependencyRepository.Delete(optionItem);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //private void ResetDefault(ContentPartFieldDefinitionRecord fieldDefinitionRecord) {
        //    var defaultItem = _optionItemRepository.Table.SingleOrDefault(x => x.ContentPartFieldDefinitionRecord == fieldDefinitionRecord && x.IsDefault);
        //    if (defaultItem != null) {
        //        defaultItem.IsDefault = false;
        //        _optionItemRepository.Update(defaultItem);
        //    }
        //}
    }
}