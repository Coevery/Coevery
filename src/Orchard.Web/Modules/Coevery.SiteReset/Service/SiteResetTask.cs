using Orchard.Core.Scheduling.Models;
using Orchard.Data;
using Orchard.Services;
using Orchard.Tasks;
using Orchard.Tasks.Scheduling;

namespace Coevery.SiteReset.Service
{
    public class SiteResetTask : IBackgroundTask
    {
        private readonly IClock _clock;
        private readonly IScheduledTaskManager _scheduledTaskManager;
        private readonly IRepository<ScheduledTaskRecord> _repository;
        public SiteResetTask(
            IScheduledTaskManager scheduledTaskManager,
            IClock clock,
            IRepository<ScheduledTaskRecord> repository){
            _scheduledTaskManager = scheduledTaskManager;
            _clock = clock;
            _repository = repository;
        }

        public void Sweep(){
            var peddingTaskCount = _repository.Count(x => x.TaskType == "SwitchTheme" || x.TaskType == "ResetSite");
            if (peddingTaskCount == 0)
            {
                _scheduledTaskManager.CreateTask("SwitchTheme", _clock.UtcNow.AddMinutes(30), null);
            }
        }
    }
}