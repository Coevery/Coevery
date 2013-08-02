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
    public class ViewDefinitionController :ApiController
    {
        private readonly IContentManager _contentManager;
        private readonly IViewPartService _viewPartService;
        public ViewDefinitionController(IContentManager iContentManager,
            IOrchardServices orchardServices,
            IViewPartService viewPartService)
        {
            _contentManager = iContentManager;
            Services = orchardServices;
            _viewPartService = viewPartService;
        }

        public IOrchardServices Services { get; private set; }

        // GET api/view/lead
        public HttpResponseMessage Get(string id) {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id))
            {
                id = pluralService.Singularize(id);
            }

            List<JObject> views = new List<JObject>();
            var projections = _contentManager.Query<ProjectionPart>().List().Where(t => t.As<TitlePart>().Title == id);
            int viewId = _viewPartService.GetProjectionId(id);
            foreach (var projectionPart in projections)
            {
                string displayName = _contentManager.Get(projectionPart.Record.QueryPartRecord.Id).As<TitlePart>().Title;
                JObject view = new JObject();
                view["ContentId"] = projectionPart.Id;
                view["EntityType"] = projectionPart.As<TitlePart>().Title;
                view["DisplayName"] = displayName;
                view["Default"] = projectionPart.Id == viewId;
                views.Add(view);
            }
            var json = JsonConvert.SerializeObject(views);
            var message = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
            return message;
        }
    }
}