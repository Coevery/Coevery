using Coevery.ContentManagement.Handlers;
using Coevery.Data;
using Coevery.Tests.ContentManagement.Models;
using Coevery.Tests.ContentManagement.Records;

namespace Coevery.Tests.ContentManagement.Handlers {
    public class LambdaPartHandler : ContentHandler {
        public LambdaPartHandler(IRepository<LambdaRecord> repository) {
            Filters.Add(new ActivatingFilter<LambdaPart>("lambda"));
            Filters.Add(StorageFilter.For(repository));

        }
    }
}
