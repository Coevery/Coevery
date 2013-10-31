using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Alias.Implementation.Holder;
using Coevery.Alias.Implementation.Storage;
using Coevery.Data;
using Coevery.Environment;
using Coevery.Tasks;
using Coevery.Logging;

namespace Coevery.Alias.Implementation.Updater {
    public class AliasHolderUpdater : ICoeveryShellEvents, IBackgroundTask {
        private readonly IAliasHolder _aliasHolder;
        private readonly IAliasStorage _storage;

        public ILogger Logger { get; set; }

        public AliasHolderUpdater(IAliasHolder aliasHolder, IAliasStorage storage) {
            _aliasHolder = aliasHolder;
            _storage = storage;
            Logger = NullLogger.Instance;
        }

        void ICoeveryShellEvents.Activated() {
            Refresh();
        }

        void ICoeveryShellEvents.Terminating() {
        }

        private void Refresh() {
            try {
                var aliases = _storage.List();
                _aliasHolder.SetAliases(aliases.Select(alias => new AliasInfo { Path = alias.Item1, Area = alias.Item2, RouteValues = alias.Item3 }));
            }
            catch (Exception ex) {
                Logger.Error(ex, "Exception during Alias refresh");
            }
        }

        public void Sweep() {
            Refresh();
        }
    }
}
