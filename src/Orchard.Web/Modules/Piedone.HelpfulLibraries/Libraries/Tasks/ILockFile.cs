using System;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks
{
    /// <summary>
    /// A file used for locking purposes
    /// </summary>
    public interface ILockFile : IDisposable, ITransientDependency
    {
        /// <summary>
        /// Tries to acquire the lock file
        /// </summary>
        /// <param name="name">Name of the lock file</param>
        /// <returns>True, if the lock could be acquired or false if not</returns>
        bool TryAcquire(string name);
    }
}
