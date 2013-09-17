using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
using Coevery.Projections.Models;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.Utility.Extensions;

namespace Coevery.Projections.Controllers {
    public class ProjectionController : ApiController {
        private readonly IContentManager _contentManager;

        public ProjectionController(
            IContentManager contentManager,
            IOrchardServices orchardServices) {
            _contentManager = contentManager;
            Services = orchardServices;
        }

        public IOrchardServices Services { get; private set; }

        public IEnumerable<JObject> Get() {
            List<JObject> re = new List<JObject>();
            var projections = _contentManager.Query<ProjectionPart>().List();
            foreach (var projectionPart in projections) {
                string displayName = _contentManager.Get(projectionPart.Record.QueryPartRecord.Id).As<TitlePart>().Title;
                JObject reObJ = new JObject();
                reObJ["ContentId"] = projectionPart.Id;
                reObJ["EntityType"] = projectionPart.As<TitlePart>().Title;
                reObJ["DisplayName"] = displayName;
                re.Add(reObJ);
            }
            return re;
        }

        public object Get(string id, int page, int rows) {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id)) {
                id = pluralService.Singularize(id);
            }

            var views = new List<JObject>();
            var queries = Services.ContentManager.Query<ListViewPart, ListViewPartRecord>("ListViewPage")
                .Where(v => v.ItemContentType == id).List();

            var query = from record in queries
                        select new {
                            ContentId = record.Id,
                            EntityType = record.ItemContentType,
                            DisplayName = record.As<TitlePart>().Title,
                            Default = record.IsDefault
                        };

            var totalRecords = query.Count();
            return new {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }

        public void Post(dynamic viewPart) {
            int id = int.Parse(viewPart.Id.ToString());
            var listView = Services.ContentManager.Get<ListViewPart>(id);

            var queries = Services.ContentManager.Query<ListViewPart, ListViewPartRecord>("ListViewPage")
                .Where(v => v.ItemContentType == listView.ItemContentType);
            var listViews = queries.List().ToList();
            foreach (var view in listViews) {
                view.IsDefault = false;
            }
            listView.IsDefault = true;
        }

        public void Delete(int id) {
            var contentItem = _contentManager.Get(id);
            _contentManager.Remove(contentItem);
        }
    }
}