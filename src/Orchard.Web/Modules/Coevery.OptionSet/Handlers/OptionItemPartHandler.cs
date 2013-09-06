using Coevery.OptionSet.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Coevery.OptionSet.Handlers {
    public class OptionItemPartHandler : ContentHandler {
        public OptionItemPartHandler(IRepository<OptionItemPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            OnInitializing<OptionItemPart>((context, part) => part.Selectable = true);
        }
    }
}