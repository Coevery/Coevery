using Coevery.Leads.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Coevery.Leads.Handlers
{
    public class LeadHandler : ContentHandler
    {
        public LeadHandler(IRepository<LeadRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}