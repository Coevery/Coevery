using System.Linq;
using Coevery.Projections.Models;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace Coevery.Projections.Handlers {
    public class LayoutPropertyPartHandler : ContentHandler {

        public LayoutPropertyPartHandler(IRepository<LayoutPropertyRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
    }
}