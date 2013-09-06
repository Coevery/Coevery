using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.DependencyInjection;
using Piedone.HelpfulLibraries.Models;

namespace Piedone.HelpfulLibraries.Tasks.Jobs
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Jobs")]
    public class JobManager : IJobManager
    {
        private readonly IRepository<JobRecord> _repository;
        private readonly IResolve<ILockFile> _lockFileResolve;
        private readonly Dictionary<IJob, JobReference> _jobReferences = new Dictionary<IJob, JobReference>();


        public JobManager(IRepository<JobRecord> repository, IResolve<ILockFile> lockFileResolve)
        {
            _repository = repository;
            _lockFileResolve = lockFileResolve;
        }


        public void CreateJob(string industry, object context, int priority)
        {
            if (String.IsNullOrEmpty(industry)) throw new ArgumentNullException("industry");

            var record = new JobRecord
            {
                Industry = industry,
                ContextDefinion = JsonConvert.SerializeObject(
                                    context,
                                    Formatting.None,
                                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }),
                Priority = priority
            };

            _repository.Create(record);
            _repository.Flush();
        }

        public IJob TakeOnlyJob(string industry)
        {
            var lockFile = _lockFileResolve.Value;
            if (!lockFile.TryAcquire("Only Job - " + industry)) return null;

            var jobRecord = CreateJobQuery(industry)
                                .Take(1)
                                .FirstOrDefault();

            if (jobRecord == null) return null;

            var job = new Job(industry, jobRecord.ContextDefinion);
            _jobReferences[job] = new JobReference { Id = jobRecord.Id, LockFile = lockFile };

            return job;
        }

        public IJob TakeJob(string industry)
        {
            var jobIdsQuery = CreateJobQuery(industry)
                                    .Select(record => record.Id)
                                    .Take(50);

            if (jobIdsQuery.Count() == 0) return null;

            var jobIds = jobIdsQuery.ToArray();
            var lockFile = _lockFileResolve.Value;
            var jobNumber = 0;

            while (jobNumber < jobIds.Length && !lockFile.TryAcquire("Job - " + industry + jobIds[jobNumber]))
            {
                jobNumber++;
            }

            // We couldn't find any open jobs in the first 50, not looking further
            if (jobNumber == jobIds.Length) return null;

            var jobRecord = _repository.Get(jobIds[jobNumber]);

            var job = new Job(industry, jobRecord.ContextDefinion);
            _jobReferences[job] = new JobReference { Id = jobRecord.Id, LockFile = lockFile };

            return job;
        }

        public void Done(IJob job)
        {
            if (job == null || !_jobReferences.ContainsKey(job)) return;

            var jobReference = _jobReferences[job];
            jobReference.LockFile.Dispose();
            _repository.Delete(_repository.Get(jobReference.Id));
            _repository.Flush();
            _jobReferences.Remove(job);
        }

        public void GiveBack(IJob job)
        {
            if (job == null || !_jobReferences.ContainsKey(job)) return;

            _jobReferences[job].LockFile.Dispose();
            _jobReferences.Remove(job);
        }

        // No need to dispose undisposed jobs' lock files, as the Dispose() on the lock files will be called by Autofac
        //public void Dispose()
        //{
        //    // Disposing every undisposed lock file
        //    foreach (var lockFile in _jobReferences.Values.Select(reference => reference.LockFile))
        //    {
        //        lockFile.Dispose();
        //    }
        //}


        private IQueryable<JobRecord> CreateJobQuery(string industry)
        {
            return _repository.Table
                                    .Where(record => record.Industry == industry)
                                    .OrderByDescending(record => record.Priority)
                                    .ThenBy(record => record.Id);
        }


        private class JobReference
        {
            public int Id { get; set; }
            public ILockFile LockFile { get; set; }
        }
    }
}
