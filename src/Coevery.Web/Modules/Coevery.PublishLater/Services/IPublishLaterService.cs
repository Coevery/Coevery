using System;
using Coevery.ContentManagement;
using Coevery.PublishLater.Models;

namespace Coevery.PublishLater.Services {
    public interface IPublishLaterService : IDependency {
        DateTime? GetScheduledPublishUtc(PublishLaterPart publishLaterPart);
        void Publish(ContentItem contentItem, DateTime scheduledPublishUtc);
    }
}