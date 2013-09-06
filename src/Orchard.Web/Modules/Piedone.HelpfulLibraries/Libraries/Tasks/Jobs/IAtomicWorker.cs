using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    /// <summary>
    /// Interface for types capable of executing atomic jobs
    /// </summary>
    public interface IAtomicWorker : IDependency
    {
        /// <summary>
        /// Works on the job
        /// </summary>
        /// <param name="job">The job object</param>
        void WorkOn(IJob job);
    }
}
