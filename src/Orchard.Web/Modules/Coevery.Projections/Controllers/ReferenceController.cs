using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Projections.Services;

namespace Coevery.Projections.Controllers {
    public class ReferenceController : ApiController {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IProjectionManager _projectionManager;
        private readonly IContentManager _contentManager;

        public ReferenceController(
            IContentDefinitionManager contentDefinitionManager,
            IProjectionManager projectionManager,
            IContentManager contentManager) {
            _contentDefinitionManager = contentDefinitionManager;
            _projectionManager = projectionManager;
            _contentManager = contentManager;
        }

        public IEnumerable<JObject> Get(string id, string fieldName) {
            var partDefinition = _contentDefinitionManager.GetPartDefinition(id);
            if (partDefinition == null) {
                return null;
            }
            var fieldDefinition = partDefinition.Fields.FirstOrDefault(x => x.Name == fieldName);
            if (fieldDefinition == null) {
                return null;
            }
            var queryId = int.Parse(fieldDefinition.Settings["ReferenceFieldSettings.QueryId"]);
            var items = new List<JObject>();
            var contentItems = _projectionManager.GetContentItems(queryId);
            foreach (var contentItem in contentItems) {
                var item = new JObject();
                item["DisplayText"] = _contentManager.GetItemMetadata(contentItem).DisplayText;
                item["Id"] = contentItem.Id;
                items.Add(item);
            }
            return items;
        }
    }
}