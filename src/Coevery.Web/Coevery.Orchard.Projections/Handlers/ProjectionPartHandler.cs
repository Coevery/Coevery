using Coevery.Data;
using Coevery.ContentManagement.Handlers;
using Coevery.Orchard.Projections.Models;

namespace Coevery.Orchard.Projections.Handlers {
    public class ProjectionPartHandler : ContentHandler {
        public ProjectionPartHandler(IRepository<ProjectionPartRecord> projecRepository) {
            Filters.Add(StorageFilter.For(projecRepository));
        }
    }
}