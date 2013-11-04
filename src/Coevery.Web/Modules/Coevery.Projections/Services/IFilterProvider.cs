using Coevery.Events;
using Coevery.Projections.Descriptors.Filter;

namespace Coevery.Projections.Services {
    public interface IFilterProvider : IEventHandler {
        void Describe(DescribeFilterContext describe);
    }
}