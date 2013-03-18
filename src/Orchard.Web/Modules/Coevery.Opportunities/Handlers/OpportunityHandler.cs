using Coevery.Opportunities.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Coevery.Opportunities.Handlers
{
    public class OpportunityHandler : ContentHandler
    {
        public OpportunityHandler(IRepository<OpportunityRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}