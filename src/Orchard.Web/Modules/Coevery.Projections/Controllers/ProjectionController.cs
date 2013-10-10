using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Coevery.Projections.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;

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

        public IEnumerable<object> Get(string id) {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id)) {
                id = pluralService.Singularize(id);
            }

            var query = Services.ContentManager.Query<ListViewPart, ListViewPartRecord>("ListViewPage")
                .Where(v => v.ItemContentType == id).List().Select(record => new {
                    ContentId = record.Id,
                    EntityType = record.ItemContentType,
                    DisplayName = record.As<TitlePart>().Title,
                    Default = record.IsDefault
                }).ToList();

            return query;
        }

        public object Get(string id, int page, int rows) {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id)) {
                id = pluralService.Singularize(id);
            }

            var query = Services.ContentManager.Query<ListViewPart, ListViewPartRecord>("ListViewPage")
                .Where(v => v.ItemContentType == id).List().Select(record => new {
                    ContentId = record.Id,
                    DisplayName = record.As<TitlePart>().Title,
                    Default = record.IsDefault
                }).ToList();

            var totalRecords = query.Count();
            return new {
                total = Convert.ToInt32(Math.Ceiling((double) totalRecords/rows)),
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