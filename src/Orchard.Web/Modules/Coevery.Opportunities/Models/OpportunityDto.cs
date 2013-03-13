using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.WebApi.Common;

namespace Coevery.Opportunities.Models
{
    public class OpportunityDto:IDto<OpportunityRecord>
    {
        public int OpportunityId { get; set; }
        public string Name { get; set; }
        public int SourceLeadId { get; set; }
        public string Description { get; set; }

        public OpportunityDto()
        {
            
        }

        public OpportunityDto(OpportunityRecord opportunity)
        {
            this.OpportunityId = opportunity.Id;
            this.Description = opportunity.Description;
            this.SourceLeadId = opportunity.LeadSourceId;
            this.Name = opportunity.Name;
        }

        public OpportunityRecord ToEntity()
        {
            return new OpportunityRecord()
            {
                Description = this.Description,
                Id = this.OpportunityId,
                LeadSourceId = this.SourceLeadId,
                Name = this.Name
            };
        }

        public object RecordId
        {
            get { return this.OpportunityId; }
            set
            {
                this.OpportunityId = Convert.ToInt32(value);
            }
        }
    }
}