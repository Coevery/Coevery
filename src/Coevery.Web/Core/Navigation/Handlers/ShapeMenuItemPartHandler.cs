using Coevery.Core.Navigation.Models;
using Coevery.Data;
using Coevery.ContentManagement.Handlers;

namespace Coevery.Core.Navigation.Handlers {
    public class ShapeMenuItemPartHandler : ContentHandler {
        public ShapeMenuItemPartHandler(IRepository<ShapeMenuItemPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}