using Orchard.WebApi.Common;

namespace Coevery.Core.Models
{
    public class OpportunityDto : IDto<Opportunity>
    {
        public int OpportunityId { get; set; }
        public string Name { get; set; }
        public int SourceLeadId { get; set; }
        public string Description { get; set; }

        public OpportunityDto()
        {

        }

        public OpportunityDto(Opportunity opportunity)
        {
            OpportunityId = opportunity.Id;
            Description = opportunity.Description;
            SourceLeadId = opportunity.SourceLeadId;
            Name = opportunity.Name;
        }

        public void UpdateEntity(Opportunity opportunity)
        {
            opportunity.Name = Name;
            opportunity.SourceLeadId = SourceLeadId;
            opportunity.Description = Description;
        }

        public int ContentId
        {
            get { return OpportunityId; }
            set { OpportunityId = value; }
        }
    }
}