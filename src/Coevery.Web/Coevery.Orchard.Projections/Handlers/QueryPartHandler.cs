using System.Linq;
using Coevery.Data;
using Coevery.ContentManagement.Handlers;
using Coevery.Localization;
using Coevery.Orchard.Projections.Models;

namespace Coevery.Orchard.Projections.Handlers {
    public class QueryPartHandler : ContentHandler {

        public QueryPartHandler(IRepository<QueryPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));

            T = NullLocalizer.Instance;

            // create a default FilterGroup on creation
            OnPublishing<QueryPart>(CreateFilterGroup);

        }

        public Localizer T { get; set; }

        private static void CreateFilterGroup(PublishContentContext ctx, QueryPart part) {
            if (!part.FilterGroups.Any()) {
                part.FilterGroups.Add(new FilterGroupRecord());
            }
        }
    }
}