using System.Linq;
using Coevery.Autoroute.Models;
using Coevery.ContentManagement;

namespace Coevery.Autoroute.Services {
    public class PathResolutionService : IPathResolutionService {
        private readonly IContentManager _contentManager;

        public PathResolutionService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public AutoroutePart GetPath(string path) {
            return _contentManager.Query<AutoroutePart, AutoroutePartRecord>()
                    .Where(part => part.DisplayAlias == path)
                    .Slice(0, 1)
                    .FirstOrDefault();
        }
    }
}
