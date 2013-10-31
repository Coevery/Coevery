using System;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Aspects;
using Coevery.ContentManagement.Utilities;

namespace Coevery.PublishLater.Models {
    public class PublishLaterPart : ContentPart<PublishLaterPart>, IPublishingControlAspect {
        private readonly LazyField<DateTime?> _scheduledPublishUtc = new LazyField<DateTime?>();
        public LazyField<DateTime?> ScheduledPublishUtc { get { return _scheduledPublishUtc; } }
    }
}
