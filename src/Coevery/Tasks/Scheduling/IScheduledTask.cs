using System;
using Coevery.ContentManagement;

namespace Coevery.Tasks.Scheduling {
    public interface IScheduledTask  {
        string TaskType { get; }
        DateTime? ScheduledUtc { get; }
        ContentItem ContentItem { get; }
    }
}