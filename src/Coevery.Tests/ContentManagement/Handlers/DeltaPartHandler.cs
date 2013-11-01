using Coevery.ContentManagement.Handlers;
using Coevery.Data;
using Coevery.Tests.ContentManagement.Models;
using Coevery.Tests.ContentManagement.Records;

namespace Coevery.Tests.ContentManagement.Handlers {
    public class DeltaPartHandler : ContentHandler {
        public DeltaPartHandler(IRepository<DeltaRecord> repository) {
            Filters.Add(new ActivatingFilter<DeltaPart>("delta"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
