using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Opportunities.Models;
using Orchard.Data;
using Orchard.WebApi.Common;

namespace Coevery.Opportunities.Controllers
{
    public class OpportunityController : RecordController<OpportunityRecord, OpportunityDto>
    {
        public OpportunityController(IRepository<OpportunityRecord> opportunityRepository)
            :base(opportunityRepository)
        {
        }

     
    }
}