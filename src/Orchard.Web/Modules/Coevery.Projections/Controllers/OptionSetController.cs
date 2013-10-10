using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using Coevery.OptionSet.Services;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement.MetaData;

namespace Coevery.Projections.Controllers
{
    public class OptionSetController : ApiController
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IOptionSetService _optionSetService;
        public OptionSetController(
            IContentDefinitionManager contentDefinitionManager,
            IOptionSetService optionSetService){
            _contentDefinitionManager = contentDefinitionManager;
            _optionSetService = optionSetService;
        }

        // GET api/<controller>
        public IEnumerable<JObject> Get(string id, string fieldName)
        {
            var partDefinition = _contentDefinitionManager.GetPartDefinition(id);
            if (partDefinition == null)
            {
                return null;
            }
            var fieldDefinition = partDefinition.Fields.FirstOrDefault(x => x.Name == fieldName);
            if (fieldDefinition == null)
            {
                return null;
            }
            var optionSetId = int.Parse(fieldDefinition.Settings["OptionSetFieldSettings.OptionSetId"]);
            var resultArr = _optionSetService.GetOptionItems(optionSetId);
            var items = new List<JObject>();
            foreach (var result in resultArr)
            {
                var item = new JObject();
                item["ID"] = result.Id;
                item["DisplayText"] = result.Name;
                items.Add(item);
            }
            return items;
        }
    }
}