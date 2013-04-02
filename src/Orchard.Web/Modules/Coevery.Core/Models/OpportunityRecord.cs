using Orchard.ContentManagement.Records;

namespace Coevery.Core.Models
{
    public class OpportunityRecord : ContentPartRecord
    {
        public virtual string Name { get; set; }
        public virtual int SourceLeadId { get; set; }
        public virtual string Description { get; set; }
    }
}