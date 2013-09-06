using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    /// <summary>
    /// Queues job executors to be run in an independent work context and transaction
    /// </summary>
    public interface IAtomicJobQueue : IDependency
    {
        /// <summary>
        /// Queues an executor to work on a job from the specified industry in an independent work context and transaction
        /// </summary>
        /// <typeparam name="TAtomicWorker">The type working on the job</typeparam>
        /// <param name="industry">"Industry", i.e. type, group of the job</param>
        void Queue<TAtomicWorker>(string industry) where TAtomicWorker : IAtomicWorker;
    }
}
