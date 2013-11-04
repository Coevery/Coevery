using Coevery.Core.Scheduling.Models;
using Coevery.Data;
using Coevery.Services;
using Coevery.Tasks;
using Coevery.Tasks.Scheduling;

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