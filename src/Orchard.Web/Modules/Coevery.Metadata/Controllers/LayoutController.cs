using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using Orchard;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;

namespace Coevery.Metadata.Controllers
{
    public class LayoutController : ApiController
    {
        private IContentDefinitionManager _contentDefinitionManager;

        public LayoutController(
            IContentDefinitionManager contentDefinitionManager
            )
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        // GET api/leads/lead/5
        public virtual object Get(string id)
        {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(id);
            if (contentTypeDefinition == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            string layout = contentTypeDefinition.Settings.ContainsKey("Layout")
                                ? contentTypeDefinition.Settings["Layout"]
                                : null;

            var layoutInfo = new
            {
                id,
                layout
            };
            return layoutInfo;
        }

        // POST api/metadata/field
        public virtual HttpResponseMessage Post(string id, FormDataCollection data) {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(id);
            if (contentTypeDefinition == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var layout = data.Get("layout");

            if (contentTypeDefinition.Settings.ContainsKey("Layout"))
            {
                contentTypeDefinition.Settings["Layout"] = layout;
            }
            else
            {
                contentTypeDefinition.Settings.Add("Layout", layout);
            }

            _contentDefinitionManager.StoreTypeDefinition(contentTypeDefinition);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}