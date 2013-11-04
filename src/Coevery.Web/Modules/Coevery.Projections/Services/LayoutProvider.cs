using Coevery.Events;
using Coevery.Projections.Descriptors.Layout;

namespace Coevery.Projections.Services {
    public interface ILayoutProvider : IEventHandler {
        void Describe(DescribeLayoutContext describe);
    }
}