using System.Collections.Generic;
using System.Linq;
using Coevery.Common.ViewModels;
using Coevery.Forms.Services;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Filter;
using Coevery.Projections.Services;
using Newtonsoft.Json;

namespace Coevery.Query.Providers {
    public class FormQueryCriteriaProvider : IQueryCriteriaProvider {
        private readonly IEnumerable<IFilterProvider> _filterProviders;
        private List<TypeDescriptor<FilterDescriptor>> _availableFilters;

        public FormQueryCriteriaProvider(
            ICoeveryServices services,
            IEnumerable<IFilterProvider> filterProviders) {
            Services = services;
            _filterProviders = filterProviders;
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
            var filterValues = Services.WorkContext.HttpContext.Request.Form["Filters"];
            if (string.IsNullOrEmpty(filterValues)) {
                return;
            }

            var contentQuery = context.Query;
            var availableFilters = AvailableFilters;
            var filters = JsonConvert.DeserializeObject<FilterData[]>(filterValues);

            foreach (var filter in filters) {
                if (filter.FormData == null || filter.FormData.Length == 0) {
                    continue;
                }

                var dictionary = filter.FormData
                    .GroupBy(d => d.Name, d => d.Value)
                    .ToDictionary(g => g.Key, g => string.Join("&", g.ToArray()));
                var state = FormParametersHelper.ToString(dictionary);

                var filterContext = new FilterContext {
                    Query = contentQuery,
                    State = FormParametersHelper.ToDynamic(state)
                };

                string category = filter.Category;
                string type = filter.Type;

                // look for the specific filter component
                var descriptor = availableFilters
                    .SelectMany(x => x.Descriptors)
                    .FirstOrDefault(x => x.Category == category && x.Type == type);

                // ignore unfound descriptors
                if (descriptor == null) {
                    continue;
                }

                // apply alteration
                descriptor.Filter(filterContext);

                contentQuery = filterContext.Query;
            }
            context.Query = contentQuery;
        }
    }
}