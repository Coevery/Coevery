using Coevery.Relationship.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;

namespace Coevery.Relationship.Handlers
{
    public class OneToManyRelationshipPartHandler : ContentHandler
    {
        public OneToManyRelationshipPartHandler(IRepository<OneToManyRelationshipRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
    }
}