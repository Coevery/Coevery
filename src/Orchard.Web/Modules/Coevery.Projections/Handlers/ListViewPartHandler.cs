using Coevery.Projections.Models;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;

namespace Coevery.Projections.Handlers {
    public class ListViewPartHandler : ContentHandler {
        public ListViewPartHandler(IRepository<ListViewPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}