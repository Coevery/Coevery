using System;
using Coevery.ContentManagement.Utilities;

namespace Coevery.ContentManagement.Aspects {
    public interface IPublishingControlAspect {
        LazyField<DateTime?> ScheduledPublishUtc { get; }
    }
}