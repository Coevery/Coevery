using Coevery.ContentManagement.Handlers;
using Coevery.Data;
using Coevery.Tests.ContentManagement.Models;
using Coevery.Tests.ContentManagement.Records;

namespace Coevery.Tests.ContentManagement.Handlers {
    class GammaPartHandler : ContentHandler {
        public GammaPartHandler(IRepository<GammaRecord> repository) {
            Filters.Add(new ActivatingFilter<GammaPart>("gamma"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
