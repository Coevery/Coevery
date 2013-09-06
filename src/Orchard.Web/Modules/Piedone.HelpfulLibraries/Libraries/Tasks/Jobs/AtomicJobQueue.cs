using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Extensions;
using Orchard.Environment.State;
using Piedone.HelpfulLibraries.DependencyInjection;
using Orchard.Exceptions;
using Orchard.Logging;
using Orchard;
using Orchard.Events;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    public interface IAtomicJobExecutor : IEventHandler
    {
        void Execute(string industry, Func<WorkContext, IAtomicWorker> executorResolver);
    }

    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Jobs")]
    public class AtomicJobQueue : IAtomicJobQueue, IAtomicJobExecutor
    {
        private readonly IProcessingEngine _processingEngine;
        private readonly ShellSettings _shellSettings;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IResolve<IJobManager> _jobManagerResolve;
        private readonly IWorkContextAccessor _wca;

        public ILogger Logger { get; set; }


        public AtomicJobQueue(
            IProcessingEngine processingEngine,
            ShellSettings shellSettings,
            IShellDescriptorManager shellDescriptorManager,
            IResolve<IJobManager> jobManagerResolve,
            IWorkContextAccessor wca)
        {
            _processingEngine = processingEngine;
            _shellSettings = shellSettings;
            _shellDescriptorManager = shellDescriptorManager;
            _jobManagerResolve = jobManagerResolve;
            _wca = wca;

            Logger = NullLogger.Instance;
        }


        public void Execute(string industry, Func<WorkContext, IAtomicWorker> executorResolver)
        {
            var jobManager = _jobManagerResolve.Value;
            IJob job = null;

            try
            {
                job = jobManager.TakeJob(industry);

                if (job == null) return;

                executorResolver(_wca.GetContext()).WorkOn(job);

                jobManager.Done(job);
            }
            catch (Exception ex)
            {
                if (ex.IsFatal()) throw;

                jobManager.GiveBack(job);
                Logger.Error(ex, "Exception during the execution of an atomic job.");
            }
        }

        public void Queue<TAtomicWorker>(string industry) where TAtomicWorker : IAtomicWorker
        {
            var shellDescriptor = _shellDescriptorManager.GetShellDescriptor();
            Func<WorkContext, IAtomicWorker> executorResolver = (workContext) => workContext.Resolve<TAtomicWorker>();

            _processingEngine.AddTask(
                _shellSettings,
                shellDescriptor,
                "IAtomicJobExecutor.Execute",
                new Dictionary<string, object> { { "industry", industry }, { "executorResolver", executorResolver } }
            );

        }
    }
}
