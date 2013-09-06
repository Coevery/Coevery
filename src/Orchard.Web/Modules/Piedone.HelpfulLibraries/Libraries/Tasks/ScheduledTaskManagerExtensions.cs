using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Tasks.Scheduling;

namespace Piedone.HelpfulLibraries.Tasks
{
    public static class ScheduledTaskManagerExtensions
    {
        public static void CreateTaskIfNew(this IScheduledTaskManager taskManager, string taskType, DateTime scheduledUtc, ContentItem contentItem)
        {
            var outdatedTaskCount = taskManager.GetTasks(taskType, DateTime.UtcNow).Count();
            var taskCount = taskManager.GetTasks(taskType).Count();
            if (taskCount != 0 && taskCount - outdatedTaskCount > 0) return;

            taskManager.CreateTask(taskType, scheduledUtc, contentItem); 
        }
    }
}
