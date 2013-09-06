using System;
using System.Threading;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks")]
    public class LockFileManager : ILockFileManager
    {
        private readonly IResolve<ILockFile> _lockFileResolve;

        public LockFileManager(IResolve<ILockFile> lockFileResolve)
        {
            _lockFileResolve = lockFileResolve;
        }

        public ILockFile TryAcquireLock(string name, int millisecondsTimeout = 4000)
        {
            int waitedMilliseconds = 0;
            var lockFile = _lockFileResolve.Value;
            bool acquired;

            while (!(acquired = lockFile.TryAcquire(name)) && waitedMilliseconds < millisecondsTimeout)
            {
                Thread.Sleep(1000);
                waitedMilliseconds += 1000;
            }

            if (acquired) return lockFile;
            else return null;
        }

        public ILockFile AcquireLock(string name, int millisecondsTimeout = 4000)
        {
            var lockResult = TryAcquireLock(name, millisecondsTimeout);
            if (lockResult != null) return lockResult;
            throw new TimeoutException("The lock on the file \"" + name + "\" couldn't be acquired in " + millisecondsTimeout.ToString() + " ms.");
        }
    }
}