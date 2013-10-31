using JetBrains.Annotations;
using Coevery.ContentManagement;
using Coevery.Logging;
using Coevery.Tasks.Scheduling;

namespace Coevery.PublishLater.Handlers {
    [UsedImplicitly]
    public class PublishingTaskHandler : IScheduledTaskHandler {
        private readonly IContentManager _contentManager;
        private readonly ICoeveryServices _coeveryServices;

        public PublishingTaskHandler(IContentManager contentManager, ICoeveryServices coeveryServices) {
            _contentManager = contentManager;
            _coeveryServices = coeveryServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Process(ScheduledTaskContext context) {
            if (context.Task.TaskType == "Publish") {
                Logger.Information("Publishing item #{0} version {1} scheduled at {2} utc",
                    context.Task.ContentItem.Id,
                    context.Task.ContentItem.Version,
                    context.Task.ScheduledUtc);

                _contentManager.Publish(context.Task.ContentItem);
            }
        }
    }
}
