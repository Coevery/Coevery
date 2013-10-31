using System;

namespace Coevery.FileSystems.Media {
    public interface IStorageFolder {
        string GetPath();
        string GetName();
        long GetSize();
        DateTime GetLastUpdated();
        IStorageFolder GetParent();
    }
}