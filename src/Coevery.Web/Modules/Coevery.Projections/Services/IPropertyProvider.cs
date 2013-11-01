using Coevery.Events;
using Coevery.Projections.Descriptors.Property;

namespace Coevery.Projections.Services {
    public interface IPropertyProvider : IEventHandler {
        void Describe(DescribePropertyContext describe);
    }
}