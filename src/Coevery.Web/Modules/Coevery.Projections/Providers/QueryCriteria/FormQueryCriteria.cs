using System.Collections.Generic;
using System.Linq;
using Coevery.Common.ViewModels;
using Coevery.ContentManagement;
using Coevery.Data;
using Coevery.Forms.Services;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Filter;
using Coevery.Projections.Models;
using Coevery.Projections.Services;
using Coevery.Tokens;
using Newtonsoft.Json;

namespace Coevery.Projections.Providers.QueryCriteria {
    public class FormQueryCriteria : IQueryCriteriaProvider {
        private readonly IEnumerable<IFilterProvider> _filterProviders;
        private List<TypeDescriptor<FilterDescriptor>> _availableFilters;
        private readonly IRepository<FilterGroupRecord> _filterGroupRepository;
        private readonly ITokenizer _tokenizer;
        private const string EmptyFilters = "[]";

        public FormQueryCriteria(
            ICoeveryServices services,
            IEnumerable<IFilterProvider> filterProviders,
            IRepository<FilterGroupRecord> filterGroupRepository,
            ITokenizer tokenizer) {
            Services = services;
            _filterProviders = filterProviders;
            _filterGroupRepository = filterGroupRepository;
            _tokenizer = tokenizer;
        }

        public ICoeveryServices Services { get; set; }

        public List<TypeDescriptor<FilterDescriptor>> AvailableFilters {
            get {
                if (_availableFilters != null) {
                    return _availableFilters;
                }

                var context = new DescribeFilterContext();
                foreach (var provider in _filterProviders) {
                    provider.Describe(context);
                }
                _availableFilters = context.Describe().ToList();
                return _availableFilters;
            }
        }

        public void Apply(QueryContext context) {
            var groupIdString = Services.WorkContext.HttpContext.Request.Form["FilterGroupId"];
            int groupId;
            if (int.TryParse(groupIdString, out groupId) && groupId != 0) {
                var filterRecords = _filterGroupRepository.Get(groupId).Filters;
                var contentQuery = context.Query;
                foreach (var record in filterRecords) {
                    var state = _tokenizer.Replace(record.State, new Dictionary<string, object>());
                    contentQuery = ApplyFilter(contentQuery, state, record.Category, record.Type);
                }
                context.Query = contentQuery;
            }
            else {
                var filterValues = Services.WorkContext.HttpContext.Request.Form["Filters"];
                if (filterValues == null || filterValues == EmptyFilters) {
                    return;
                }
                var filters = JsonConvert.DeserializeObject<FilterData[]>(filterValues);
                var contentQuery = context.Query;

                foreach (var filter in filters) {
                    if (filter.FormData == null || filter.FormData.Length == 0) {
                        continue;
                    }

                    var dictionary = filter.FormData
                        .GroupBy(d => d.Name, d => d.Value)
                        .ToDictionary(g => g.Key, g => string.Join("&", g.ToArray()));

                    string state = FormParametersHelper.ToString(dictionary);
                    contentQuery = ApplyFilter(contentQuery, state, filter.Category, filter.Type);
                }
                context.Query = contentQuery;
            }
        }

        private IHqlQuery ApplyFilter(IHqlQuery query, string state, string category, string type) {
            // look for the specific filter component
            var descriptor = AvailableFilters
                .SelectMany(x => x.Descriptors)
                .FirstOrDefault(x => x.Category == category && x.Type == type);

            if (descriptor != null) {
                var filterContext = new FilterContext {
                    Query = query,
                    State = FormParametersHelper.ToDynamic(state)
                };
                // apply alteration
                descriptor.Filter(filterContext);
                query = filterContext.Query;
            }

            return query;
        }
    }
}