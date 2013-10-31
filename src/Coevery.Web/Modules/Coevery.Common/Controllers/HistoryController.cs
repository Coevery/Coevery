using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Common.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coevery.ContentManagement;
using System.Linq;

namespace Coevery.Common.Controllers {
    public class HistoryController : ApiController {
        private readonly IContentManager _contentManager;
        private readonly ICompareContentService _compareObjectService;

        public HistoryController(IContentManager contentManager,
            ICompareContentService compareObjectService) {
            _contentManager = contentManager;
            _compareObjectService = compareObjectService;
        }

        // GET api/leads/lead
        public HttpResponseMessage Get(int id) {
            if (id <= 0) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var histories = new List<JObject>();
            var contentItems = _contentManager.GetAllVersions(id).ToList();
            for (int itemIndex = 0; itemIndex < contentItems.Count() - 1; itemIndex++) {
                var result = _compareObjectService.CompareContent(contentItems[itemIndex], contentItems[itemIndex + 1]);
                if (!result) {
                    foreach (var defferent in _compareObjectService.Defferences) {
                        var history = new JObject();
                        history["Date"] = string.Empty;
                        history["User"] = string.Empty;
                        history["Action"] = defferent.Value;
                        histories.Add(history);
                    }
                }
            }
            var json = JsonConvert.SerializeObject(histories);
            var message = new HttpResponseMessage {Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")};

            return message;
        }
    }
}