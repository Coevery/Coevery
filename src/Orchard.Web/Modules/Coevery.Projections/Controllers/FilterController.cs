using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Coevery.Projections.Models;
using Coevery.Projections.ViewModels;
using Newtonsoft.Json.Linq;
using Orchard.Data;
using Orchard.Forms.Services;
using Orchard.Projections.Models;

namespace Coevery.Projections.Controllers {
    public class FilterController : ApiController {
        private readonly IRepository<EntityFilterRecord> _entityFilterRepository;
        private readonly IRepository<FilterGroupRecord> _filterGroupRepository;

        public FilterController(
            IRepository<EntityFilterRecord> entityFilterRepository,
            IRepository<FilterGroupRecord> filterGroupRepository) {
            _entityFilterRepository = entityFilterRepository;
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
                filterGroup["Id"] = entityFilterRecord.Id;
                filterGroup["FilterGroupId"] = entityFilterRecord.FilterGroupRecord.Id;
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

        public IEnumerable<JObject> Post(string id, FilterViewModel model) {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            if (pluralService.IsPlural(id)) {
                id = pluralService.Singularize(id);
            }

            var groupRecord = new FilterGroupRecord();
                _filterGroupRepository.Create(groupRecord);
                _entityFilterRepository.Create(new EntityFilterRecord {
                    EntityName = id,
                    FilterGroupRecord = groupRecord,
                    Title = model.Title
                });

            foreach (var filter in model.Filters) {
                if (filter.FormData.Length == 0) {
                    continue;
                }
                var record = new FilterRecord {
                    Category = id + "ContentFields",
                    Type = filter.Type,
                };
                var dictionary = new Dictionary<string, string>();
                foreach (var data in filter.FormData) {
                    if (dictionary.ContainsKey(data.Name)) {
                        dictionary[data.Name] += "&" + data.Value;
                    }
                    else {
                        dictionary.Add(data.Name, data.Value);
                    }
                }
                record.State = FormParametersHelper.ToString(dictionary);
                groupRecord.Filters.Add(record);
            }

            return Get(id);
        }

        public void Delete(string id, int filterId) {
            var entityFilterRecord = _entityFilterRepository.Get(filterId);
            if (entityFilterRecord == null) {
                return;
            }
            entityFilterRecord.FilterGroupRecord.Filters.Clear();
            _filterGroupRepository.Delete(entityFilterRecord.FilterGroupRecord);
            _entityFilterRepository.Delete(entityFilterRecord);
        }
    }
}