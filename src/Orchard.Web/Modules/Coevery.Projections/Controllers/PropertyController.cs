using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Projections.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Coevery.Core;

namespace Coevery.Projections.Controllers {
    public class PropertyController : ApiController {
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;

        public PropertyController(IContentManager contentManager,
            IProjectionManager projectionManager) {
            _contentManager = contentManager;
            _projectionManager = projectionManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<object> Get(int id) {
            if (id <= 0)
                return null;
            var properties = GetDescriptors(id).Select(x => new {FieldName = x.Descriptor.Type, DisplayName = x.Descriptor.Name.Text});
            return properties;
        }

        public HttpResponseMessage Get(string id, int viewId) {
            if (string.IsNullOrEmpty(id)) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }

            var firstColumn = new JObject();
            firstColumn["name"] = "ContentId";
            firstColumn["label"] = T("Content Id").Text;
            firstColumn["hidden"] = true;

            var columns = new List<JObject> { firstColumn };
            var properties = GetDescriptors(viewId).Select(x => x.Property).ToList();
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
                //column["sortable"] = false;
                columns.Add(column);
            }

            var json = JsonConvert.SerializeObject(columns);
            var message = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };

            return message;
        }

        private IEnumerable<FieldDescriptor> GetDescriptors(int projectionId) {
            if (projectionId == -1)
                return Enumerable.Empty<FieldDescriptor>();

            var projectionPart = _contentManager.Get<ProjectionPart>(projectionId);
            var queryPartRecord = projectionPart.Record.QueryPartRecord;

            if (queryPartRecord.Layouts.Count == 0)
                return Enumerable.Empty<FieldDescriptor>();
            var allFielDescriptors = _projectionManager.DescribeProperties().ToList();
            var fieldDescriptors = queryPartRecord.Layouts[0].Properties.OrderBy(p => p.Position).Select(p => allFielDescriptors.SelectMany(x => x.Descriptors).Select(d => new FieldDescriptor {Descriptor = d, Property = p}).FirstOrDefault(x => x.Descriptor.Category == p.Category && x.Descriptor.Type == p.Type)).ToList();
            return fieldDescriptors;
        }

        class FieldDescriptor {
            public PropertyDescriptor Descriptor { get; set; }
            public PropertyRecord Property { get; set; }
        }
    }
}