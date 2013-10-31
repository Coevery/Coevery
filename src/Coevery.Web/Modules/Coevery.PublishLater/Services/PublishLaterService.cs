using System;
using Coevery.ContentManagement;
using Coevery.Core.Contents;
using Coevery.Localization;
using Coevery.PublishLater.Models;
using Coevery.Tasks.Scheduling;

namespace Coevery.PublishLater.Services {
    public class PublishLaterService : IPublishLaterService {
        private readonly IPublishingTaskManager _publishingTaskManager;

        public PublishLaterService(
            ICoeveryServices services,  
            IPublishingTaskManager publishingTaskManager) {
            Services = services;
            _publishingTaskManager = publishingTaskManager;
            T = NullLocalizer.Instance;
        }

        public ICoeveryServices Services { get; set; }
        public Localizer T { get; set; }

        void IPublishLaterService.Publish(ContentItem contentItem, DateTime scheduledPublishUtc) {
            if (!Services.Authorizer.Authorize(Permissions.PublishContent, contentItem, T("Couldn't publish selected content.")))
                return;

            _publishingTaskManager.Publish(contentItem, scheduledPublishUtc);
        }

        DateTime? IPublishLaterService.GetScheduledPublishUtc(PublishLaterPart publishLaterPart) {
            IScheduledTask task = _publishingTaskManager.GetPublishTask(publishLaterPart.ContentItem);
            return (task == null ? null : task.ScheduledUtc);
        }
    }
}