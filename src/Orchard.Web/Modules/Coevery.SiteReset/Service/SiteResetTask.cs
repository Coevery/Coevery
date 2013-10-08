using System;
using Orchard.Core.Scheduling.Models;
using Orchard.Data;
using Orchard.Services;
using Orchard.Tasks;
using Orchard.Tasks.Scheduling;
using Orchard.Themes.Services;

namespace Coevery.SiteReset.Service
{
    public class SiteResetTask : IBackgroundTask
    {
        private readonly IClock _clock;
        private readonly IScheduledTaskManager _scheduledTaskManager;
        private readonly IRepository<ScheduledTaskRecord> _repository;
        private readonly IThemeService _themeService;
        private readonly ISiteThemeService _siteThemeService;
        public SiteResetTask(IScheduledTaskManager scheduledTaskManager,
            IClock clock,
            IRepository<ScheduledTaskRecord> repository,
            IThemeService themeService,
            ISiteThemeService siteThemeService){
            _scheduledTaskManager = scheduledTaskManager;
            _clock = clock;
            _repository = repository;
            _themeService = themeService;
            _siteThemeService = siteThemeService;
        }

        public void Sweep(){
            var peddingTaskCount = _repository.Count(x => x.ScheduledUtc > _clock.UtcNow && x.TaskType == "ResetSite"
                ||x.ScheduledUtc > _clock.UtcNow && x.TaskType == "SwitchTheme");
            if (peddingTaskCount == 0)
            {
                _themeService.EnableThemeFeatures("Mooncake");
                _siteThemeService.SetSiteTheme("Mooncake");
                _scheduledTaskManager.CreateTask("SwitchTheme", _clock.UtcNow.AddMinutes(29), null);
                _scheduledTaskManager.CreateTask("ResetSite", _clock.UtcNow.AddMinutes(30), null);
            }
        }
    }
}