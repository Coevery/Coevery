using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Leads.Models;
using Orchard.Data;
using Orchard.WebApi.Common;

namespace Coevery.Leads.Controllers
{
    public class LeadController : RecordController<LeadRecord,LeadDto>
    {
        public LeadController(IRepository<LeadRecord> leadRepository)
            :base(leadRepository)
        {
            
        }
    }
}