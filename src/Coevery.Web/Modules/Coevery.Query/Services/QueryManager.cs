using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Data;
using Coevery.Forms.Services;
using Coevery.Localization;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Filter;
using Coevery.Projections.Descriptors.SortCriterion;
using Coevery.Projections.Models;
using Coevery.Projections.Services;
using Coevery.Query.Providers;
using Coevery.Tokens;

namespace Coevery.Query.Services {
    public class QueryManager : IQueryManager {
        private readonly ITokenizer _tokenizer;
        private readonly IEnumerable<IFilterProvider> _filterProviders;
        private readonly IEnumerable<ISortCriterionProvider> _sortCriterionProviders;
        private readonly IContentManager _contentManager;
        private readonly IRepository<QueryPartRecord> _queryRepository;
        private readonly IEnumerable<IQueryCriteriaProvider> _queryCriteriaProviders;

        public QueryManager(
            ITokenizer tokenizer,
            IEnumerable<IFilterProvider> filterProviders,
            IEnumerable<ISortCriterionProvider> sortCriterionProviders,
            IContentManager contentManager,
            IRepository<QueryPartRecord> queryRepository,
            IEnumerable<IQueryCriteriaProvider> queryCriteriaProviders) {
            _tokenizer = tokenizer;
            _filterProviders = filterProviders;
            _sortCriterionProviders = sortCriterionProviders;
            _contentManager = contentManager;
            _queryRepository = queryRepository;
            _queryCriteriaProviders = queryCriteriaProviders;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<TypeDescriptor<FilterDescriptor>> DescribeFilters() {
            var context = new DescribeFilterContext();

            foreach (var provider in _filterProviders) {
                provider.Describe(context);
            }
            return context.Describe();
        }

        public IEnumerable<TypeDescriptor<SortCriterionDescriptor>> DescribeSortCriteria() {
            var context = new DescribeSortCriterionContext();

            foreach (var provider in _sortCriterionProviders) {
                provider.Describe(context);
            }
            return context.Describe();
        }

        public int GetCount(int queryId) {
            var queryRecord = _queryRepository.Get(queryId);

            if (queryRecord == null) {
                throw new ArgumentException("queryId");
            }

            // aggregate the result for each group query

            return GetContentQueries(queryRecord, Enumerable.Empty<SortCriterionRecord>())
                .Sum(contentQuery => contentQuery.Count());
        }

        public IEnumerable<ContentItem> GetContentItems(int queryId, string contentTypeName, int skip = 0, int count = 0) {
            var availableSortCriteria = DescribeSortCriteria().ToList();

            var queryRecord = _queryRepository.Get(queryId);

            if (queryRecord == null) {
                throw new ArgumentException("queryId");
            }

            var contentQuery = _contentManager.HqlQuery().ForVersion(VersionOptions.Published);

            foreach (var criteriaProvider in _queryCriteriaProviders) {
                var context = new QueryContext {
                    Query = contentQuery,
                    QueryPartRecord = queryRecord,
                    ContentTypeName = contentTypeName
                };

                criteriaProvider.Apply(context);

                contentQuery = context.Query;
            }

            // iterate over each sort criteria to apply the alterations to the query object
            foreach (var sortCriterion in queryRecord.SortCriteria) {
                var sortCriterionContext = new SortCriterionContext {
                    Query = contentQuery,
                    State = FormParametersHelper.ToDynamic(sortCriterion.State)
                };

                string category = sortCriterion.Category;
                string type = sortCriterion.Type;

                // look for the specific filter component
                var descriptor = availableSortCriteria.SelectMany(x => x.Descriptors).FirstOrDefault(x => x.Category == category && x.Type == type);

                // ignore unfound descriptors
                if (descriptor == null) {
                    continue;
                }

                // apply alteration
                descriptor.Sort(sortCriterionContext);

                contentQuery = sortCriterionContext.Query;
            }

            return contentQuery.Slice(skip, count);
        }

        public IEnumerable<IHqlQuery> GetContentQueries(QueryPartRecord queryRecord, IEnumerable<SortCriterionRecord> sortCriteria) {
            var availableFilters = DescribeFilters().ToList();
            var availableSortCriteria = DescribeSortCriteria().ToList();

            // pre-executing all groups 
            foreach (var group in queryRecord.FilterGroups) {
                var contentQuery = _contentManager.HqlQuery().ForVersion(VersionOptions.Published);

                // iterate over each filter to apply the alterations to the query object
                foreach (var filter in group.Filters) {
                    var tokenizedState = _tokenizer.Replace(filter.State, new Dictionary<string, object>());
                    var filterContext = new FilterContext {
                        Query = contentQuery,
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

                    contentQuery = filterContext.Query;
                }

                // iterate over each sort criteria to apply the alterations to the query object
                foreach (var sortCriterion in sortCriteria) {
                    var sortCriterionContext = new SortCriterionContext {
                        Query = contentQuery,
                        State = FormParametersHelper.ToDynamic(sortCriterion.State)
                    };

                    string category = sortCriterion.Category;
                    string type = sortCriterion.Type;

                    // look for the specific filter component
                    var descriptor = availableSortCriteria
                        .SelectMany(x => x.Descriptors)
                        .FirstOrDefault(x => x.Category == category && x.Type == type);

                    // ignore unfound descriptors
                    if (descriptor == null) {
                        continue;
                    }

                    // apply alteration
                    descriptor.Sort(sortCriterionContext);

                    contentQuery = sortCriterionContext.Query;
                }


                yield return contentQuery;
            }
        }
    }
}