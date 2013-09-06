using Orchard.Caching;
using Orchard.Environment.Extensions;

namespace Piedone.Combinator.EventHandlers
{
    [OrchardFeature("Piedone.Combinator")]
    public class CombinatorEventMonitor : ICombinatorEventMonitor
    {
        private readonly ISignals _signals;

        private const string _configurationChangedSignal = "Piedone.Combinator.ConfigurationChangedSignal";
        private const string _cacheEmptiedSignal = "Piedone.Combinator.CacheEmptiedSignal";

        public CombinatorEventMonitor(ISignals signals)
        {
            _signals = signals;
        }

        public void MonitorConfigurationChanged(IAcquireContext acquireContext)
        {
            acquireContext.Monitor(_signals.When(_configurationChangedSignal));
        }

        public void MonitorCacheEmptied(IAcquireContext acquireContext)
        {
            acquireContext.Monitor(_signals.When(_cacheEmptiedSignal));
        }

        public void ConfigurationChanged()
        {
            _signals.Trigger(_configurationChangedSignal);
        }

        public void CacheEmptied()
        {
            _signals.Trigger(_cacheEmptiedSignal);
        }
    }
}