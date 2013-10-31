using System.Collections.Generic;
using Coevery.Environment.Extensions.Models;

namespace Coevery.Environment.Extensions.Folders {
    public interface IExtensionFolders {
        IEnumerable<ExtensionDescriptor> AvailableExtensions();
    }
}