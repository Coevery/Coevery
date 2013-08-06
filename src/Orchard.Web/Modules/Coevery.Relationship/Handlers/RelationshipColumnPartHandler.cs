using Coevery.Relationship.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;

namespace Coevery.Relationship.Handlers
{
    public class RelationshipColumnPartHandler:ContentHandler
    {
        public RelationshipColumnPartHandler(IRepository<RelationshipColumnRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
    }
}