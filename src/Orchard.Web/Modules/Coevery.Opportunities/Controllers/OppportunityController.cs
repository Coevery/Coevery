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

        // GET api/opportunities/opportunity
        public IEnumerable<OpportunityDto> GetOpportunity() {
           return GetRecords();
        }

        // GET api/opportunities/opportunity/5
        public OpportunityDto GetOpportunity(int id) {
            return GetRecord(id);
        }

        // PUT api/opportunities/opportunity/5
        public HttpResponseMessage PutOpportunity(int id, OpportunityDto opportunityDto) {
            return PutRecord(id, opportunityDto);
        }

        // POST api/opportunities/opportunity
        public HttpResponseMessage PostOpportunity(OpportunityDto opportunityDto) {
            return PostRecord(opportunityDto);
        }

        // DELETE api/opportunities/opportunity/5
        public HttpResponseMessage DeleteOpportunity(int id) {
            return DeleteRecord(id);
        }
    }
}