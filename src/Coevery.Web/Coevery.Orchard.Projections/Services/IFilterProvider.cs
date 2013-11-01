using Coevery.Events;
using Coevery.Orchard.Projections.Descriptors.Filter;

namespace Coevery.Orchard.Projections.Services {
    public interface IFilterProvider : IEventHandler {
        void Describe(DescribeFilterContext describe);
    }
}