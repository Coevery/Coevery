using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.Opportunities.Models
{
    public class OpportunityDto
    {
        public int OpportunityId { get; set; }
        public string Name { get; set; }
        public int LeadSourceId { get; set; }
        public string Description { get; set; }

        public OpportunityDto(OpportunityRecord opportunity)
        {
            this.OpportunityId = opportunity.Id;
            this.Description = opportunity.Description;
            this.LeadSourceId = opportunity.LeadSourceId;
            this.Name = opportunity.Name;
        }

        public OpportunityRecord ToEntity()
        {
            return new OpportunityRecord()
            {
                Description = this.Description,
                Id = this.OpportunityId,
                LeadSourceId = this.LeadSourceId,
                Name = this.Name
            };
        }
    }
}