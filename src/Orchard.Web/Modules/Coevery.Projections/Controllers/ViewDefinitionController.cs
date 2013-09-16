using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Net.Http;
using System.Web.Http;
using Coevery.Projections.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Projections.Models;
using System.Linq;

namespace Coevery.Projections.Controllers {
    public class ViewDefinitionController : ApiController {
        public ViewDefinitionController(IOrchardServices orchardServices) {
            Services = orchardServices;
        }

        public IOrchardServices Services { get; private set; }

        // GET api/view/lead
        public HttpResponseMessage Get(string id) {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id)) {
                id = pluralService.Singularize(id);
            }

            var views = new List<JObject>();
            var queries = Services.ContentManager.Query<ListViewPart, ListViewPartRecord>("ListViewPage")
                .Where(v => v.ItemContentType == id);
            var listViews = queries.List().ToList();
            foreach (var record in listViews) {
                var view = new JObject();
                view["ContentId"] = record.Id;
                view["EntityType"] = record.ItemContentType;
                view["DisplayName"] = record.As<TitlePart>().Title;
                view["Default"] = record.IsDefault;
                views.Add(view);
            }

            var json = JsonConvert.SerializeObject(views);
            var message = new HttpResponseMessage {Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")};
            return message;
        }
    }
}