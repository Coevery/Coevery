using System.Web.Hosting;

namespace Coevery.FileSystems.VirtualPath {
    public interface ICustomVirtualPathProvider {
        VirtualPathProvider Instance { get; }
    }
}