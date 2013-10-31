using System.IO;
using Coevery.Environment.Extensions.Models;

namespace Coevery.Packaging.Services {
    public interface IPackageBuilder : IDependency {
        Stream BuildPackage(ExtensionDescriptor extensionDescriptor);
    }
}