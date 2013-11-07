using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Coevery.Common.Extensions;
using Coevery.Projections.Models;
using Coevery;
using Coevery.ContentManagement;
using Coevery.Core.Title.Models;

namespace Coevery.Projections.Controllers {
    public class ProjectionController : ApiController {
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;

        public ProjectionController(
            IContentManager contentManager,
            IContentDefinitionExtension contentDefinitionExtension,
            ICoeveryServices coeveryServices) {
            _contentDefinitionExtension = contentDefinitionExtension;
            _contentManager = contentManager;
            Services = coeveryServices;
        }

        public ICoeveryServices Services { get; private set; }

        public IEnumerable<object> Get(string id) {
            id = _contentDefinitionExtension.GetEntityNameFromCollectionName(id);

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
            var query = Services.ContentManager.Query<ListViewPart, ListViewPartRecord>("ListViewPage")
                .Where(v => v.ItemContentType == id).List().Select(record => new {
                    ContentId = record.Id,
                    DisplayName = record.As<TitlePart>().Title,
                    Default = record.IsDefault
                }).ToList();

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