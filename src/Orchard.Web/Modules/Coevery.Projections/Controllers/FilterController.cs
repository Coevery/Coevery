using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using Coevery.Projections.Models;
using Newtonsoft.Json.Linq;
using Orchard.Data;
using Orchard.Forms.Services;
using Orchard.Projections.Models;
using Orchard.Projections.Services;

namespace Coevery.Projections.Controllers {
    public class FilterController : ApiController {
        private readonly IRepository<EntityFilterRecord> _entityFilterRepository;
        private readonly IRepository<FilterRecord> _filterRepository;
        private readonly IProjectionManager _projectionManager;

        public FilterController(
            IRepository<EntityFilterRecord> entityFilterRepository,
            IProjectionManager projectionManager,
            IRepository<FilterRecord> filterRepository) {
            _entityFilterRepository = entityFilterRepository;
            _projectionManager = projectionManager;
            _filterRepository = filterRepository;
        }

        public IEnumerable<JObject> Get(string id) {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id)) {
                id = pluralService.Singularize(id);
            }
            var re = new List<JObject>();
            var entityFilterRecords = _entityFilterRepository.Table.Where(x => x.EntityName == id);
            foreach (var entityFilterRecord in entityFilterRecords) {
                var groupObj = new JObject();
                groupObj["Id"] = entityFilterRecord.FilterGroupRecord.Id;
                groupObj["Title"] = entityFilterRecord.Title;
                var filters = new JArray();
                foreach (var filter in entityFilterRecord.FilterGroupRecord.Filters) {
                    var filterObj = new JObject();
                    filterObj["Type"] = filter.Type;
                    filterObj["State"] = FormParametersHelper.ToDynamic(filter.State);
                    filters.Add(filter);
                }
                groupObj["Filters"] = filters;
                re.Add(groupObj);
            }
            return re;
        }

        private void Test() {
            var filterRecord = new FilterRecord {
                Category = "LeadContentFields",
                Type = "Lead.Topic.",
                Position = 0,
                Description = "Test Filter"
            };
            var state = new {
                Value = "Lead 1"
            };
            filterRecord.State = FormParametersHelper.ToString(state);
            _filterRepository.Create(filterRecord);
            var groupRecord = new FilterGroupRecord();
            groupRecord.Filters.Add(filterRecord);
            _entityFilterRepository.Create(new EntityFilterRecord {
                EntityName = "Lead",
                FilterGroupRecord = groupRecord
            });
        }
    }
}