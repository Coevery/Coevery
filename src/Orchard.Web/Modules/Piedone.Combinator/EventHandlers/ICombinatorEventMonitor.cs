using Orchard.Caching;

namespace Piedone.Combinator.EventHandlers
{
    public interface ICombinatorEventMonitor : ICombinatorEventHandler
    {
        void MonitorConfigurationChanged(IAcquireContext acquireContext);
        void MonitorCacheEmptied(IAcquireContext acquireContext);
    }
}
