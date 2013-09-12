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
        private readonly IRepository<FilterGroupRecord> _filterGroupRepository;
        private readonly IProjectionManager _projectionManager;

        public FilterController(
            IRepository<EntityFilterRecord> entityFilterRepository,
            IProjectionManager projectionManager,
            IRepository<FilterGroupRecord> filterGroupRepository) {
            _entityFilterRepository = entityFilterRepository;
            _projectionManager = projectionManager;
            _filterGroupRepository = filterGroupRepository;
        }

        public IEnumerable<JObject> Get(string id) {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id)) {
                id = pluralService.Singularize(id);
            }
            var entityFilters = new List<JObject>();
            var entityFilterRecords = _entityFilterRepository.Table.Where(x => x.EntityName == id);
            foreach (var entityFilterRecord in entityFilterRecords) {
                var filterGroup = new JObject();
                filterGroup["Id"] = entityFilterRecord.FilterGroupRecord.Id;
                filterGroup["Title"] = entityFilterRecord.Title;
                var filters = new JArray();
                foreach (var filterRecord in entityFilterRecord.FilterGroupRecord.Filters) {
                    var filter = new JObject();
                    filter["Type"] = filterRecord.Type;
                    filter["State"] = FormParametersHelper.ToDynamic(filterRecord.State);
                    filters.Add(filter);
                }
                filterGroup["Filters"] = filters;
                entityFilters.Add(filterGroup);
            }
            return entityFilters;
        }

        private void Test() {
            var filterRecord = new FilterRecord {
                Category = "LeadContentFields",
                Type = "Lead.Topic.",
                Position = 0,
            };
            var state = new {
                Value = "Lead 1"
            };
            filterRecord.State = FormParametersHelper.ToString(state);
            var groupRecord = new FilterGroupRecord();
            _filterGroupRepository.Create(groupRecord);
            groupRecord.Filters.Add(filterRecord);
            _entityFilterRepository.Create(new EntityFilterRecord {
                EntityName = "Lead",
                FilterGroupRecord = groupRecord,
                Title = "Test Filter"
            });
        }
    }
}