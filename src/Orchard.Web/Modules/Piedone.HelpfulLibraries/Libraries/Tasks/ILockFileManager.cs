using Orchard.Caching;

namespace Piedone.HelpfulLibraries.Tasks
{
    /// <summary>
    /// Locking services bases on the usage of lock files that will work in any server environment
    /// and independent from the lifecycle of database transactions.
    /// </summary>
    public interface ILockFileManager : IVolatileProvider
    {
        /// <summary>
        /// Tries to acquire a lock with the specified parameters
        /// </summary>
        /// <param name="name">Unique name of the lock</param>
        /// <param name="millisecondsTimeout">Milliseconds to wait for the lock before timing out</param>
        /// <returns>The ILockFile instance on success or null if the lock couldn't be acquired.</returns>
        ILockFile TryAcquireLock(string name, int millisecondsTimeout = 4000);

        /// <summary>
        /// Tries to acquire a lock with the specified parameters
        /// </summary>
        /// <param name="name">Unique name of the lock</param>
        /// <param name="millisecondsTimeout">Milliseconds to wait for the lock before timing out</param>
        /// <returns>The ILockFile instance on success.</returns>
        /// <exception cref="System.TimeoutException">Thrown if the lock couldn't be acquired.</exception>
        ILockFile AcquireLock(string name, int millisecondsTimeout = 4000);
    }
}
