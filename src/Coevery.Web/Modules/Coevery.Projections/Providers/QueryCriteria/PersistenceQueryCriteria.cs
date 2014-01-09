using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Forms.Services;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Filter;
using Coevery.Projections.Models;
using Coevery.Projections.Services;
using Coevery.Tokens;

namespace Coevery.Projections.Providers.QueryCriteria {
    public class PersistenceQueryCriteria : IQueryCriteriaProvider {
        private readonly IEnumerable<IFilterProvider> _filterProviders;
        private readonly ITokenizer _tokenizer;
        private readonly IContentManager _contentManager;
        private List<TypeDescriptor<FilterDescriptor>> _availableFilters;

        public PersistenceQueryCriteria(
            IEnumerable<IFilterProvider> filterProviders,
            ITokenizer tokenizer,
            IContentManager contentManager) {
            _filterProviders = filterProviders;
            _tokenizer = tokenizer;
            _contentManager = contentManager;
        }

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
            var queryRecord = context.QueryPartRecord;
            if (queryRecord.FilterGroups.Count == 0) {
                return;
            }

            if (queryRecord.FilterGroups.Count == 1) {
                context.Query = ApplyFilterGroup(queryRecord.FilterGroups.First(), context.Query);
            }
            else {
                var contentItems = new List<ContentItem>();
                foreach (var group in queryRecord.FilterGroups) {
                    var contentQuery = _contentManager.HqlQuery().ForVersion(VersionOptions.Published);
                    contentQuery = ApplyFilterGroup(group, contentQuery);
                    contentItems.AddRange(contentQuery.List());
                }
                var ids = contentItems.Select(c => c.Id).ToArray();
                context.Query = context.Query.Where(alias => alias.Named("ci"), x => x.InG("Id", ids));
            }
        }

        private IHqlQuery ApplyFilterGroup(FilterGroupRecord groupRecord, IHqlQuery query) {
            var availableFilters = AvailableFilters;
            foreach (var filter in groupRecord.Filters) {
                var tokenizedState = _tokenizer.Replace(filter.State, new Dictionary<string, object>());
                var filterContext = new FilterContext {
                    Query = query,
                    State = FormParametersHelper.ToDynamic(tokenizedState)
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

                query = filterContext.Query;
            }
            return query;
        }
    }
}