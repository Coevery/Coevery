using System;
using System.Linq;
using System.Web.Mvc;
using Orchard.Mvc.Filters;
using Orchard.Services;
using Orchard.Tasks.Scheduling;

namespace Coevery.SiteReset.Filters
{
    public class ResetSiteFilter :FilterProvider, IActionFilter
    {
        private readonly IClock _clock;
        private readonly IScheduledTaskManager _scheduledTaskManager;
        public ResetSiteFilter(
            IClock clock,
            IScheduledTaskManager scheduledTaskManager) {
            _clock = clock;
            _scheduledTaskManager = scheduledTaskManager;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_scheduledTaskManager.GetTasks("ResetSite").Any()) return;
            try {
                _scheduledTaskManager.DeleteTasks(null, item => item.TaskType == "ResetSite");
                _scheduledTaskManager.CreateTask("ResetSite", _clock.UtcNow.AddMinutes(30), null);
            }
            catch (Exception ex){
                
            }
        }
    }
}