using Coevery.Data;
using Coevery.ContentManagement.Handlers;
using Coevery.Projections.Models;

namespace Coevery.Projections.Handlers {
    public class NavigationQueryPartHandler : ContentHandler {
        public NavigationQueryPartHandler(IRepository<NavigationQueryPartRecord> navigationQueryRepository) {
            Filters.Add(StorageFilter.For(navigationQueryRepository));
        }
    }
}