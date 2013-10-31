using Coevery.Events;
using Coevery.Orchard.Projections.Descriptors.Layout;

namespace Coevery.Orchard.Projections.Services {
    public interface ILayoutProvider : IEventHandler {
        void Describe(DescribeLayoutContext describe);
    }
}