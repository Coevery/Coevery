using Coevery.ContentManagement.Handlers;
using Coevery.Data;
using Coevery.Perspectives.Models;

namespace Coevery.Perspectives.Handlers
{
    public class PerspectivePartHandler : ContentHandler {

        public PerspectivePartHandler(IRepository<PerspectivePartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
