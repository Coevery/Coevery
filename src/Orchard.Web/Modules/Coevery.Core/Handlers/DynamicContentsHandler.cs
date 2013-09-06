using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.Records;
using Orchard.Data;

namespace Coevery.Core.Handlers {
    public abstract class DynamicContentsHandler<TRecord> : ContentHandler where TRecord : ContentPartRecord, new() {
        public DynamicContentsHandler(IRepository<TRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}