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
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Mvc.ViewEngines;
using Orchard.Mvc.ViewEngines.ThemeAwareness;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using System.Linq;
using Orchard.Tokens;

namespace Coevery.Core.Controllers
{
    public class ColumnDefinitionController :ApiController
    {
        private readonly IContentManager _contentManager;
        private readonly IViewPartService _projectionService;
        private readonly ILayoutAwareViewEngine _layoutAwareViewEngine;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ColumnDefinitionController(IContentManager iContentManager,
            IOrchardServices orchardServices,
            IViewPartService projectionService, 
            ILayoutAwareViewEngine layoutAwareViewEngine, 
            IWorkContextAccessor workContextAccessor)
        {
            _contentManager = iContentManager;
            Services = orchardServices;
            _projectionService = projectionService;
            _layoutAwareViewEngine = layoutAwareViewEngine;
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; private set; }

        // GET api/leads/lead
        public HttpResponseMessage Get(string id) {
            if (string.IsNullOrEmpty(id)) {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            id = pluralService.Singularize(id);

            int projectionId = _projectionService.GetProjectionId(id);
            var properties = GetProperties(projectionId);

            var columns = new List<JObject>();
            string linkCellTemplae = RenderView("GridTemplate", "LinkCellTemplate");
            foreach (var property in properties) {
                var column = new JObject();
                var filedName = property.GetFiledName();
                column["field"] = filedName;
                column["displayName"] = T(property.Description).Text;
                if (property.LinkToContent) {
                    column["cellTemplate"] = linkCellTemplae;
                }
                columns.Add(column);
            }
            var json = JsonConvert.SerializeObject(columns);
            var message = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };

            return message;
        }

        private string RenderView(string controllerName, string viewName) {
            HttpContextBase contextBase = new HttpContextWrapper(HttpContext.Current);

            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);
            routeData.Values.Add("area", "Coevery.Core");
            routeData.DataTokens.Add("area", "Coevery.Core");
            routeData.DataTokens.Add("IWorkContextAccessor", _workContextAccessor);

            var controllerContext = new ControllerContext(contextBase, routeData, new EmptyController());

            var razorViewResult = _layoutAwareViewEngine.FindView(controllerContext, viewName, "", false);

            var writer = new StringWriter();
            var viewContext = new ViewContext(controllerContext, razorViewResult.View, new ViewDataDictionary(), new TempDataDictionary(), writer);
            razorViewResult.View.Render(viewContext, writer);

            return writer.ToString();
        }

        private class EmptyController : ControllerBase {
            protected override void ExecuteCore() { }
        }

        private IEnumerable<PropertyRecord> GetProperties(int projectionId) {
            IList<PropertyRecord> properties = new List<PropertyRecord>();
            if (projectionId == -1) 
                return properties;

            var projectionItem = _contentManager.Get(projectionId, VersionOptions.Latest);
            var projectionPart = projectionItem.As<ProjectionPart>();
            var queryPartRecord = projectionPart.Record.QueryPartRecord;

            if (queryPartRecord.Layouts.Count == 0) 
                return properties;
            properties = queryPartRecord.Layouts[0].Properties;
            return properties;
        }

        private class ViewDataContainer : IViewDataContainer {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}