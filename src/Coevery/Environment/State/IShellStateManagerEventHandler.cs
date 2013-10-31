using Coevery.Events;

namespace Coevery.Environment.State {
    public interface IShellStateManagerEventHandler : IEventHandler {
        void ApplyChanges();
    }
}