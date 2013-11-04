using Coevery.Events;

namespace Coevery.Environment.Configuration {
    public interface IShellSettingsManagerEventHandler : IEventHandler {
        void Saved(ShellSettings settings);
    }
}