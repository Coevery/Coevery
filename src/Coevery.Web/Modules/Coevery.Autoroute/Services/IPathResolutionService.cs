using Coevery.Autoroute.Models;

namespace Coevery.Autoroute.Services {

    public interface IPathResolutionService : IDependency {
        AutoroutePart GetPath(string path);
    }
}
