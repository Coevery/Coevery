using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Coevery.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Mvc.ViewEngines;
using Orchard.Mvc.ViewEngines.ThemeAwareness;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using System.Linq;
using Orchard.Projections.Services;
using Orchard.Tokens;

namespace Coevery.Core.Controllers
{
    public class HistoryController :ApiController
    {
        private readonly IContentManager _contentManager;
        private readonly ICompareContentService _compareObjectService;
        private readonly IProjectionManager _projectionManager;
        public HistoryController(IContentManager contentManager,
            ICompareContentService compareObjectService, 
            IProjectionManager projectionManager)
        {
            _contentManager = contentManager;
            _compareObjectService = compareObjectService;
            _projectionManager = projectionManager;
        }


        // GET api/leads/lead
        public HttpResponseMessage Get(int id)
        {
            if (id <= 0) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var histories = new List<JObject>();
            var contentItems = _contentManager.GetAllVersions(id).ToList();
            for (int itemIndex = 0; itemIndex < contentItems.Count() - 1; itemIndex++)
            {
                var result = _compareObjectService.CompareContent(contentItems[itemIndex], contentItems[itemIndex + 1]);
                if (!result)
                {
                    foreach (var defferent in _compareObjectService.Defferences)
                    {
                        var history = new JObject();
                        history["Date"] = string.Empty;
                        history["User"] = string.Empty;
                        history["Action"] = defferent.Value;
                        histories.Add(history);
                    }
                }
            }
            var json = JsonConvert.SerializeObject(histories);
            var message = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };

            return message;
        }
    }
}