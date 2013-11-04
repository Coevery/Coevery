using System;

namespace Coevery.FileSystems.LockFile
{
    public interface ILockFile : IDisposable {
        void Release();
    }
}
