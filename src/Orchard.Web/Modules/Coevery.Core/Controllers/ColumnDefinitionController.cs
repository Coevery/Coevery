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

namespace Coevery.Core.Controllers {
    public class ColumnDefinitionController : ApiController {
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;
        private readonly ITemplateViewService _templateViewService;
        public ColumnDefinitionController(IContentManager iContentManager,
            IOrchardServices orchardServices,
            IProjectionManager projectionManager,
            ITemplateViewService templateViewService) {
            _contentManager = iContentManager;
            Services = orchardServices;
            _projectionManager = projectionManager;
            _templateViewService = templateViewService;
            T = NullLocalizer.Instance;
        }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; private set; }

        // GET api/leads/lead
        public HttpResponseMessage Get(string id, int viewId) {
            if (string.IsNullOrEmpty(id)) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }

            var firstColumn = new JObject();
            firstColumn["name"] = "ContentId";
            firstColumn["label"] = T("Content Id").Text;
            firstColumn["hidden"] = true;

            var columns = new List<JObject> { firstColumn };
            var properties = GetProperties(viewId);
            //string linkCellTemplae = _templateViewService.RenderView("Coevery.Core", "GridTemplate", "LinkCellTemplate");
            foreach (var property in properties) {
                var column = new JObject();
                var filedName = property.GetFiledName();
                column["name"] = filedName;
                column["label"] = T(property.Description).Text;
                if (property.LinkToContent) {
                    var formatOpt = new JObject();
                    formatOpt["hasView"] = true;
                    column["formatter"] = "cellLinkTemplate";
                    column["formatoptions"] = formatOpt;
                }
                columns.Add(column);
            }

            var json = JsonConvert.SerializeObject(columns);
            var message = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };

            return message;
        }

        private IEnumerable<PropertyRecord> GetProperties(int projectionId) {
            IList<PropertyRecord> properties = new List<PropertyRecord>();
            if (projectionId == -1)
                return properties;

            var projectionPart = _contentManager.Get<ProjectionPart>(projectionId);
            var queryPartRecord = projectionPart.Record.QueryPartRecord;

            if (queryPartRecord.Layouts.Count == 0)
                return properties;
            var allFielDescriptors = _projectionManager.DescribeProperties().ToList();
            var fieldDescriptors = queryPartRecord.Layouts[0].Properties.OrderBy(p => p.Position).Select(p => allFielDescriptors.SelectMany(x => x.Descriptors).Select(d => new { Descriptor = d, Property = p }).FirstOrDefault(x => x.Descriptor.Category == p.Category && x.Descriptor.Type == p.Type)).ToList();
            properties = fieldDescriptors.Where(c => c != null).Select(c => c.Property).ToList();
            return properties;
        }
    }
}