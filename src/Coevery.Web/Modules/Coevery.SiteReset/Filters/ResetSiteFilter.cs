using System.Web.Mvc;
using Coevery.Core.Scheduling.Models;
using Coevery.Data;
using Coevery.Mvc.Filters;
using Coevery.Services;
using Coevery.Tasks.Scheduling;

namespace Coevery.SiteReset.Filters
{
    public class ResetSiteFilter :FilterProvider, IActionFilter
    {
        private readonly IClock _clock;
        private readonly IRepository<ScheduledTaskRecord> _repository;
        public ResetSiteFilter(
            IClock clock,
            IRepository<ScheduledTaskRecord> repository)
        {
            _clock = clock;
            _repository = repository;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext) {
            var task = _repository.Get(item => item.TaskType == "SwitchTheme");
            if (task==null) return;
            task.ScheduledUtc=_clock.UtcNow.AddMinutes(30);
            _repository.Update(task);
        }
    }
}