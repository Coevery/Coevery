using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Coevery.Data;
using Coevery.Logging;

namespace Coevery.Tasks {

    public interface IBackgroundService : IDependency {
        void Sweep();
    }

    [UsedImplicitly]
    public class BackgroundService : IBackgroundService {
        private readonly IEnumerable<IBackgroundTask> _tasks;
        private readonly ITransactionManager _transactionManager;

        public BackgroundService(IEnumerable<IBackgroundTask> tasks, ITransactionManager transactionManager) {
            _tasks = tasks;
            _transactionManager = transactionManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Sweep() {
            foreach(var task in _tasks) {
                try {
                    _transactionManager.RequireNew();
                    task.Sweep();
                }
                catch (Exception e) {
                    _transactionManager.Cancel();
                    Logger.Error(e, "Error while processing background task");
                }
                
            }
        }
    }
}
