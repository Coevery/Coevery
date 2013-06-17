using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using System.Linq;
using Orchard.Tokens;

namespace Coevery.Core.Controllers
{
    public class NGgridController :ApiController
    {
        private IContentManager _contentManager;
        private readonly IViewPartService _projectionService;

        public NGgridController(IContentManager iContentManager,
            IOrchardServices orchardServices,
            IViewPartService projectionService)
        {
            _contentManager = iContentManager;
            Services = orchardServices;
            _projectionService = projectionService;
            T = NullLocalizer.Instance;
        }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; private set; }

        // GET api/leads/lead
        public IEnumerable<object> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            id = pluralService.Singularize(id);

            int viewId = _projectionService.GetProjectionId(id);
            var columns = GetViewColumns(viewId);
            int index = 0;
            List<object> ngColumns = new List<object>();
            string actionTemplate = "<div style=\"margin-top: 5px;margin-left: 5px;\"><span class=\"span2\">"+
                "<a ng-click=\"edit(row.getProperty(col.field))\">Eidt</a></span><span class=\"span2\"></span>"+
                "<span class=\"span2\"><a ng-click=\"delete(row.getProperty(col.field))\">Remove</a></span></div>";
            ngColumns.Add(new { field = "ContentId", displayName = "Actions", width = 150, cellTemplate = actionTemplate });
            ngColumns.Add(new { field = "ContentId", displayName = T("Id").Text });

            string moduleName = id;
            if (pluralService.IsSingular(moduleName))
            {
                moduleName = pluralService.Pluralize(moduleName);
            }
            string cellVarTemp = "<div><a href =\"Coevery#/" + moduleName + "/{{{{row.entity.ContentId}}}}\" class=\"ngCellText\">{{{{row.entity.{0}}}}}</a></div>';";
            foreach (var col in columns)
            {
                string cellTemplae = string.Empty;
                if (col.LinkToContent) {
                    cellTemplae = string.Format(cellVarTemp, col.Description);
                }
                ngColumns.Add(new { field = col.Description, displayName = T(col.Description).Text, cellTemplate = cellTemplae });
                index++;
            }
            return ngColumns;
        }

        private IEnumerable<PropertyRecord> GetViewColumns(int viewId)
        {
            List<PropertyRecord> re = new List<PropertyRecord>();
            if (viewId == -1) return re;
            var projectionItem = _contentManager.Get(viewId, VersionOptions.Latest);
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryPartRecord = projectionPart.Record.QueryPartRecord;

            if (queryPartRecord.Layouts.Count == 0) return re;
            var properties = queryPartRecord.Layouts[0].Properties;
            re.AddRange(properties);
            return re;
        }
    }
}