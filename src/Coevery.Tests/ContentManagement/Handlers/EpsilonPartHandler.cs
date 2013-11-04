using Coevery.ContentManagement.Handlers;
using Coevery.Data;
using Coevery.Tests.ContentManagement.Models;

namespace Coevery.Tests.ContentManagement.Handlers {
    public class EpsilonPartHandler : ContentHandler {
        public EpsilonPartHandler(IRepository<EpsilonRecord> repository) {
            Filters.Add(new ActivatingFilter<EpsilonPart>("gamma"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
