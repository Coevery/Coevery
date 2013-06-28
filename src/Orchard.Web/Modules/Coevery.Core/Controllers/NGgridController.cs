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
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.DisplayManagement.Implementation;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Mvc.ViewEngines;
using Orchard.Mvc.ViewEngines.ThemeAwareness;
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
        private readonly IDisplayManager _displayManager;
        private readonly IViewPartService _projectionService;
        private readonly ILayoutAwareViewEngine _layoutAwareViewEngine;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IProjectionManager _projectionManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public NGgridController(IContentManager iContentManager,

            IOrchardServices orchardServices,
            ILayoutAwareViewEngine layoutAwareViewEngine,
            IViewPartService projectionService, 
            IDisplayManager displayManager, 
            IWorkContextAccessor workContextAccessor, 
            IProjectionManager projectionManager, IContentDefinitionManager contentDefinitionManager)
        {
            _contentManager = iContentManager;
            Services = orchardServices;
            _layoutAwareViewEngine = layoutAwareViewEngine;
            _projectionService = projectionService;
            _displayManager = displayManager;
            _workContextAccessor = workContextAccessor;
            _projectionManager = projectionManager;
            _contentDefinitionManager = contentDefinitionManager;
            T = NullLocalizer.Instance;
        }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; private set; }

        class EmptyController : ControllerBase
        {
            protected override void ExecuteCore() { }
        }


        private string GetGridTemplate(string viewName, ViewDataDictionary viewData)
        {
            HttpContextBase contextBase = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();
            routeData.Values.Add("controller", "controller");
            routeData.DataTokens.Add("IWorkContextAccessor", _workContextAccessor);
            var controllerContext = new System.Web.Mvc.ControllerContext(contextBase, routeData, new EmptyController());
            var razorViewResult = _layoutAwareViewEngine.FindView(controllerContext, viewName, string.Empty, false);
            var writer = new StringWriter();
            var viewContext = new ViewContext(controllerContext, razorViewResult.View,
                                              new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
            viewContext.GetWorkContext();
            razorViewResult.View.Render(viewContext, writer);
            return writer.ToString();
        }

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
            string actionTemplate = GetGridTemplate("CreateAction",new ViewDataDictionary());
           
            List<object> ngColumns = new List<object>();
            ngColumns.Add(new { field = "ContentId", displayName = "Actions", width = 150, cellTemplate = actionTemplate });
            ngColumns.Add(new { field = "ContentId", displayName = T("Id").Text });

            string moduleName = id;
            if (pluralService.IsSingular(moduleName))
            {
                moduleName = pluralService.Pluralize(moduleName);
            }
            ViewDataDictionary viewData = new ViewDataDictionary();
            viewData.Add("ModuleName", moduleName);
            string cellVarTemp = GetGridTemplate("GridLink", viewData); //"<div><a href =\"Coevery#/" + moduleName + "/{{{{row.entity.ContentId}}}}\" class=\"ngCellText\">{{{{row.entity.{0}}}}}</a></div>';";
            foreach (var col in columns) {
                string cellTemplae = string.Empty;
                if (col.LinkToContent) {
                    cellTemplae = string.Format(cellVarTemp, col.Type);
                }
                ngColumns.Add(new { field = col.Type, displayName = T(col.Description).Text, cellTemplate = cellTemplae });
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