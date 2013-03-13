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
       
        public IEnumerable<LeadDto> GetLeads() 
        {
            return GetRecords();
        }
        
        public LeadDto GetLead(int id)
        {
            return GetRecord(id);
        }
        
        public HttpResponseMessage PutLead(int id, LeadDto leadDto) 
        {
            return PutRecord(id, leadDto);
        }
       
        public HttpResponseMessage PostLead(LeadDto leadDto)
        {
            return PostRecord(leadDto);
        }
      
        public HttpResponseMessage DeleteLead(int id) 
        {
            return DeleteRecord(id);
        }
    }
}