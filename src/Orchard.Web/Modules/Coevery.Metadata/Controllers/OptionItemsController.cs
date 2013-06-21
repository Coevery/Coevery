using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Fields.Records;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Orchard.Localization;

namespace Coevery.Metadata.Controllers {
    public class OptionItemsController : ApiController {
        private readonly IRepository<OptionItemRecord> _optionItemRepository;
        private readonly IRepository<ContentPartDefinitionRecord> _partDefinitionRepository;

        public OptionItemsController(
            IRepository<OptionItemRecord> optionItemRepository,
            IRepository<ContentPartDefinitionRecord> partDefinitionRepository) {
            _optionItemRepository = optionItemRepository;
            _partDefinitionRepository = partDefinitionRepository;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/metadata/OptionItems
        public object Get(string entityName, string fieldName) {
            var partDefinition = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (partDefinition == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var fieldDefinitionRecord = partDefinition.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == fieldName);
            if (fieldDefinitionRecord == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var items = _optionItemRepository.Table.Where(x => x.ContentPartFieldDefinitionRecord == fieldDefinitionRecord).Select(x => new { x.Id, x.IsDefault, x.Value });
            return items;
        }

        // PUT api/metadata/OptionItems?Id=1
        public HttpResponseMessage Put(int id, OptionItemRecord optionItem) {
            var optionItemRecord = _optionItemRepository.Table.SingleOrDefault(x => x.Id == id);
            if (optionItemRecord == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            ResetDefault(optionItemRecord.ContentPartFieldDefinitionRecord);
            optionItemRecord.Value = optionItem.Value;
            optionItemRecord.IsDefault = optionItem.IsDefault;
            _optionItemRepository.Update(optionItemRecord);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/metadata/OptionItems
        public HttpResponseMessage PostContent(string entityName, string fieldName, OptionItemRecord optionItem) {
            var partDefinition = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (partDefinition == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var fieldDefinitionRecord = partDefinition.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == fieldName);
            if (fieldDefinitionRecord == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            ResetDefault(fieldDefinitionRecord);
            optionItem.ContentPartFieldDefinitionRecord = fieldDefinitionRecord;
            _optionItemRepository.Create(optionItem);
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        // DELETE api/metadata/OptionItems?Id=1
        public HttpResponseMessage Delete(int id) {
            var optionItem = _optionItemRepository.Table.SingleOrDefault(x => x.Id == id);
            if (optionItem == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            _optionItemRepository.Delete(optionItem);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void ResetDefault(ContentPartFieldDefinitionRecord fieldDefinitionRecord) {
            var defaultItem = _optionItemRepository.Table.SingleOrDefault(x => x.ContentPartFieldDefinitionRecord == fieldDefinitionRecord && x.IsDefault);
            if (defaultItem != null) {
                defaultItem.IsDefault = false;
                _optionItemRepository.Update(defaultItem);
            }
        }
    }
}