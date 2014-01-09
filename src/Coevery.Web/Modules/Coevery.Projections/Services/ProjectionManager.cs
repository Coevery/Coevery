using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Data;
using Coevery.Forms.Services;
using Coevery.Projections.Descriptors;
using Coevery.Localization;
using Coevery.Projections.Descriptors.Property;
using Coevery.Projections.Descriptors.Filter;
using Coevery.Projections.Descriptors.Layout;
using Coevery.Projections.Descriptors.SortCriterion;
using Coevery.Projections.Models;

namespace Coevery.Projections.Services {
    public class ProjectionManager : IProjectionManager{
        private readonly IEnumerable<IFilterProvider> _filterProviders;
        private readonly IEnumerable<ISortCriterionProvider> _sortCriterionProviders;
        private readonly IEnumerable<ILayoutProvider> _layoutProviders;
        private readonly IEnumerable<IPropertyProvider> _propertyProviders;
        private readonly IContentManager _contentManager;
        private readonly IRepository<QueryPartRecord> _queryRepository;
        private readonly IEnumerable<IQueryCriteriaProvider> _queryCriteriaProviders;

        public ProjectionManager(IEnumerable<IFilterProvider> filterProviders,
            IEnumerable<ISortCriterionProvider> sortCriterionProviders,
            IEnumerable<ILayoutProvider> layoutProviders,
            IEnumerable<IPropertyProvider> propertyProviders,
            IContentManager contentManager,
            IRepository<QueryPartRecord> queryRepository, 
            IEnumerable<IQueryCriteriaProvider> queryCriteriaProviders) {
            _filterProviders = filterProviders;
            _sortCriterionProviders = sortCriterionProviders;
            _layoutProviders = layoutProviders;
            _propertyProviders = propertyProviders;
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

        public IEnumerable<TypeDescriptor<LayoutDescriptor>> DescribeLayouts() {
            var context = new DescribeLayoutContext();

            foreach (var provider in _layoutProviders) {
                provider.Describe(context);
            }
            return context.Describe();
        }

        public IEnumerable<TypeDescriptor<PropertyDescriptor>> DescribeProperties() {
            var context = new DescribePropertyContext();

            foreach (var provider in _propertyProviders) {
                provider.Describe(context);
            }
            return context.Describe();
        }

        public FilterDescriptor GetFilter(string category, string type) {
            return DescribeFilters()
                .SelectMany(x => x.Descriptors)
                .FirstOrDefault(x => x.Category == category && x.Type == type);
        }

        public SortCriterionDescriptor GetSortCriterion(string category, string type) {
            return DescribeSortCriteria()
                .SelectMany(x => x.Descriptors)
                .FirstOrDefault(x => x.Category == category && x.Type == type);
        }

        public LayoutDescriptor GetLayout(string category, string type) {
            return DescribeLayouts()
                .SelectMany(x => x.Descriptors)
                .FirstOrDefault(x => x.Category == category && x.Type == type);
        }

        public PropertyDescriptor GetProperty(string category, string type) {
            return DescribeProperties()
                .SelectMany(x => x.Descriptors)
                .FirstOrDefault(x => x.Category == category && x.Type == type);
        }

        public int GetCount(int queryId, string contentTypeName = null) {

            var queryRecord = _queryRepository.Get(queryId);

            if (queryRecord == null) {
                throw new ArgumentException("queryId");
            }

            // aggregate the result for each group query

            return GetContentQuery(queryRecord, contentTypeName).Count();
        }

        public IEnumerable<ContentItem> GetContentItems(int queryId, int skip = 0, int count = 0, string contentTypeName = null) {
            var availableSortCriteria = DescribeSortCriteria().ToList();

            var queryRecord = _queryRepository.Get(queryId);

            if (queryRecord == null) {
                throw new ArgumentException("queryId");
            }

            var contentQuery = GetContentQuery(queryRecord, contentTypeName);

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

        public IHqlQuery GetContentQuery(QueryPartRecord queryRecord, string contentTypeName) {
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

            return contentQuery;
        }
    }
}