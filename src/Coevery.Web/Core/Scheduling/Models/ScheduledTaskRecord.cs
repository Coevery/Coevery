using System;
using Coevery.ContentManagement.Records;

namespace Coevery.Core.Scheduling.Models {
    public class ScheduledTaskRecord {
        public virtual int Id { get; set; }
        public virtual string TaskType { get; set; }
        public virtual DateTime? ScheduledUtc { get; set; }
        public virtual ContentItemVersionRecord ContentItemVersionRecord { get; set; }
    }
}