using Coevery.Projections.Models;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;

namespace Coevery.Projections.Handlers {
    public class LayoutPropertyPartHandler : ContentHandler {
        public LayoutPropertyPartHandler(IRepository<LayoutPropertyRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}