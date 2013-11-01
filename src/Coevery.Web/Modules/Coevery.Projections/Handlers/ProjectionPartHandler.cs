using Coevery.Data;
using Coevery.ContentManagement.Handlers;
using Coevery.Projections.Models;

namespace Coevery.Projections.Handlers {
    public class ProjectionPartHandler : ContentHandler {
        public ProjectionPartHandler(IRepository<ProjectionPartRecord> projecRepository) {
            Filters.Add(StorageFilter.For(projecRepository));
        }
    }
}