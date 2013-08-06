using Coevery.Relationship.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;

namespace Coevery.Relationship.Handlers
{
    public class ManyToManyRelationshipPartHandler : ContentHandler
    {
        public ManyToManyRelationshipPartHandler(IRepository<ManyToManyRelationshipRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
    }
}