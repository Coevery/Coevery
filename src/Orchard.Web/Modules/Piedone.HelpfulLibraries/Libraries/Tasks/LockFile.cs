using System;
using System.IO;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.Media;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks")]
    public class LockFile : ILockFile
    {
        private readonly IStorageProvider _storageProvider;
        private string _name;
        private bool _isDisposed = false;
        private bool _isAcquired = false;

        private const string _folder = "HelpfulLibraries/Tasks/LockFiles/";

        public LockFile(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public bool TryAcquire(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            using (var stream = new MemoryStream())
            {
                var canAcquire = _storageProvider.TrySaveStream(MakeFilePath(name), stream);

                if (canAcquire)
                {
                    _name = name;
                    _isAcquired = true;
                }

                return canAcquire;
            }
        }

        // This will be called at least by Autofac when the request ends
        public void Dispose()
        {
            if (_isDisposed || !_isAcquired) return;

            _isDisposed = true;
            // Could throw exception e.g. if the file was deleted. This should not happen.
            _storageProvider.DeleteFile(MakeFilePath(_name));
        }

        private static string MakeFilePath(string name)
        {
            return _folder + name + ".lock";
        }
    }
}
