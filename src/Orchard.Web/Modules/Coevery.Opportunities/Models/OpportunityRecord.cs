using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.Opportunities.Models
{
    public class OpportunityRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int LeadSourceId { get; set; }
        public virtual string Description { get; set; }
    }
}