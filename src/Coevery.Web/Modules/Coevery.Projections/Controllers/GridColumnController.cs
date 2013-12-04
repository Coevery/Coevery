using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Projections.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Projections.Descriptors.Property;
using Coevery.Projections.Models;

namespace Coevery.Projections.Controllers {
    public class GridColumnController : ApiController {
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;

        public GridColumnController(IContentManager contentManager,
            IProjectionManager projectionManager) {
            _contentManager = contentManager;
            _projectionManager = projectionManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public HttpResponseMessage Get(string id, int viewId) {
            if (string.IsNullOrEmpty(id)) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }

            var keyColumn = new JObject();
            keyColumn["name"] = "ContentId";
            keyColumn["label"] = T("Content Id").Text;
            keyColumn["hidden"] = true;
            keyColumn["key"] = true;

            IEnumerable<FieldDescriptor> properties = Enumerable.Empty<FieldDescriptor>();
            LayoutRecord layoutRecord = null;
            #region Load Properties

            var projectionPart = _contentManager.Get<ProjectionPart>(viewId);
            if (projectionPart != null) {
                var queryPartRecord = projectionPart.Record.QueryPartRecord;
                if (queryPartRecord.Layouts.Any())
                    layoutRecord = queryPartRecord.Layouts[0];
            }

            if (layoutRecord != null) {
                var allFielDescriptors = _projectionManager.DescribeProperties().ToList();
                properties = layoutRecord.Properties.OrderBy(p => p.Position)
                    .Select(p => allFielDescriptors.SelectMany(x => x.Descriptors)
                        .Select(d => new FieldDescriptor {Descriptor = d, Property = p}).FirstOrDefault(x => x.Descriptor.Category == p.Category && x.Descriptor.Type == p.Type)).ToList();
            }

            #endregion


            var columns = new List<JObject> {keyColumn};

            foreach (var property in properties.Select(x => x.Property)) {
                var column = new JObject();
                var filedName = property.GetFieldName();
                column["name"] = filedName;
                column["label"] = T(property.Description).Text;
                if (property.LinkToContent) {
                    var formatOpt = new JObject();
                    formatOpt["hasView"] = true;
                    column["formatter"] = "cellLinkTemplate";
                    column["formatoptions"] = formatOpt;
                }
                //column["sortable"] = false;
                columns.Add(column);
            }
            var gridOptions = new JObject();
            gridOptions["colModel"] = JToken.FromObject(columns);
            if (layoutRecord != null && layoutRecord.Groups.Any()) {
                gridOptions["grouping"] = true;
                
                var groupingView = new JObject();
                groupingView["groupField"] = JToken.FromObject(layoutRecord.Groups.OrderBy(g => g.Position).Select(g => g.GroupProperty.GetFieldName()));
                groupingView["groupOrder"] = JToken.FromObject(layoutRecord.Groups.OrderBy(g => g.Position).Select(g => g.Sort));
                groupingView["groupColumnShow"] = false;

                gridOptions["groupingView"] = groupingView;
            }
            var json = JsonConvert.SerializeObject(gridOptions);
            var message = new HttpResponseMessage {Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")};

            return message;
        }

        class FieldDescriptor {
            public PropertyDescriptor Descriptor { get; set; }
            public PropertyRecord Property { get; set; }
        }
    }
}