using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    /// <summary>
    /// Deals with managing tasks that should be completed once
    /// </summary>
    public interface IJobManager : IDependency
    {
        /// <summary>
        /// Creates a new job in the specified industy
        /// </summary>
        /// <param name="industry">"Industry", i.e. type, group of the job</param>
        /// <param name="context">An arbitrary serializable context object</param>
        /// <param name="priority">Priority affects the order jobs are retrieved in</param>
        void CreateJob(string industry, object context, int priority);

        /// <summary>
        /// Takes the job with the highest priority (or the oldest if there are only jobs with the same priority) of the industry. 
        /// Till this job is not done or given back other calls to this method won't receive a job.
        /// </summary>
        /// <param name="industry">"Industry", i.e. type, group of the job</param>
        /// <returns>The job with the highest priority (or the oldest) or null if there are no jobs or the only one is already worked on.</returns>
        IJob TakeOnlyJob(string industry);

        /// <summary>
        /// Takes the job with the highest priority (or the oldest if there are only jobs with the same priority) available in the industry. 
        /// Subsequent calls to this method will get the next jobs, even if the calls were simultaneous.
        /// </summary>
        /// <param name="industry">"Industry", i.e. type, group of the job</param>
        /// <returns>The job with the highest priority (or the oldest) available in the industy or null if there are no available jobs left.</returns>
        IJob TakeJob(string industry);

        /// <summary>
        /// Marks the job as done, removing it from the set of available jobs
        /// </summary>
        /// <param name="job">The job object</param>
        void Done(IJob job);

        /// <summary>
        /// Frees up the job so it can be taken again.
        /// </summary>
        /// <param name="job">The job object</param>
        void GiveBack(IJob job);
    }
}
