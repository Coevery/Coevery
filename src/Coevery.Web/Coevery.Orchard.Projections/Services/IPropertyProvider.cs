using Coevery.Events;
using Coevery.Orchard.Projections.Descriptors.Property;

namespace Coevery.Orchard.Projections.Services {
    public interface IPropertyProvider : IEventHandler {
        void Describe(DescribePropertyContext describe);
    }
}