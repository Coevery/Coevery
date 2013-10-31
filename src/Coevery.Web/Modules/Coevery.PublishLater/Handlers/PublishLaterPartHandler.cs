using Coevery.ContentManagement.Handlers;
using Coevery.PublishLater.Models;
using Coevery.PublishLater.Services;
using Coevery.Tasks.Scheduling;

namespace Coevery.PublishLater.Handlers {
    public class PublishLaterPartHandler : ContentHandler {
        private readonly IPublishLaterService _publishLaterService;

        public PublishLaterPartHandler(
            IPublishLaterService publishLaterService,
            IPublishingTaskManager publishingTaskManager) {
            _publishLaterService = publishLaterService;

            OnLoading<PublishLaterPart>((context, part) => LazyLoadHandlers(part));
            OnVersioning<PublishLaterPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));
            OnRemoved<PublishLaterPart>((context, part) => publishingTaskManager.DeleteTasks(part.ContentItem));
        }

        protected void LazyLoadHandlers(PublishLaterPart part) {
            part.ScheduledPublishUtc.Loader((value) => _publishLaterService.GetScheduledPublishUtc(part));
        }
    }
}