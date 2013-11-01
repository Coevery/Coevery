using Coevery.Data;
using Coevery.ContentManagement.Handlers;
using Coevery.Orchard.Projections.Models;

namespace Coevery.Orchard.Projections.Handlers {
    public class NavigationQueryPartHandler : ContentHandler {
        public NavigationQueryPartHandler(IRepository<NavigationQueryPartRecord> navigationQueryRepository) {
            Filters.Add(StorageFilter.For(navigationQueryRepository));
        }
    }
}