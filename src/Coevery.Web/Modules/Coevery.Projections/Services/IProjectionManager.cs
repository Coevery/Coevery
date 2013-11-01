using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.Projections.Descriptors;
using Coevery.Projections.Descriptors.Property;
using Coevery.Projections.Descriptors.Filter;
using Coevery.Projections.Descriptors.Layout;
using Coevery.Projections.Descriptors.SortCriterion;

namespace Coevery.Projections.Services {
    public interface IProjectionManager : IDependency {
        IEnumerable<TypeDescriptor<FilterDescriptor>> DescribeFilters();
        IEnumerable<TypeDescriptor<SortCriterionDescriptor>> DescribeSortCriteria();
        IEnumerable<TypeDescriptor<LayoutDescriptor>> DescribeLayouts();
        IEnumerable<TypeDescriptor<PropertyDescriptor>> DescribeProperties();

        FilterDescriptor GetFilter(string category, string type);
        SortCriterionDescriptor GetSortCriterion(string category, string type);
        LayoutDescriptor GetLayout(string category, string type);
        PropertyDescriptor GetProperty(string category, string type);

        IEnumerable<ContentItem> GetContentItems(int queryId, int skip = 0, int count = 0);
        int GetCount(int queryId);
    }

}