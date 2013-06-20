using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Coevery.Fields.Records;
using Coevery.Metadata.Services;
using Coevery.Metadata.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Orchard.Localization;
using Orchard.WebApi.Common;
using Orchard.Utility.Extensions;

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
        public IEnumerable<object> Get(string fieldName, string entityName) {
            var partDefinition = _partDefinitionRepository.Table.SingleOrDefault(x => x.Name == entityName);
            if (partDefinition == null) {
                //return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var fieldDefinitionRecord = partDefinition.ContentPartFieldDefinitionRecords.SingleOrDefault(x => x.Name == fieldName);
            if (fieldDefinitionRecord == null) {
                //return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var items = _optionItemRepository.Table.Where(x => x.ContentPartFieldDefinitionRecord == fieldDefinitionRecord).Select(x => new { x.Id, x.IsDefault, x.Value });
            return items.ToList();
            //return null;
        }

        // DELETE api/metadata/field/name
        public virtual HttpResponseMessage Delete(string name, string parentname) {
            //_contentDefinitionService.RemoveFieldFromPart(name, parentname);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}