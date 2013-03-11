using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Leads.Models;
using Orchard.Data;

namespace Coevery.Leads.Controllers
{
    public class LeadController : ApiController
    {
        private IRepository<LeadRecord> _leadRepository;

        public LeadController(IRepository<LeadRecord> leadRepository)
        {
            _leadRepository = leadRepository;
        }

        // GET api/leads/lead
        public IEnumerable<LeadDto> GetLeads()
        {
            var reDtos = new List<LeadDto>();
            var re = _leadRepository.Table.ToList();
            reDtos.AddRange(re.Select(lead => new LeadDto(lead)));
            return reDtos;
        }

        // GET api/leads/lead/5
        public LeadDto GetLead(int id)
        {
            var lead = _leadRepository.Get(id);
            if (lead == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return new LeadDto(lead);
        }

        // PUT api/leads/lead/5
        public HttpResponseMessage PutLead(int id, LeadDto leadDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var oldLead = _leadRepository.Get(id);
            if (oldLead == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var newlead = leadDto.ToEntity();
            _leadRepository.Copy(newlead, oldLead);

            try
            {
                _leadRepository.Flush();
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/leads/lead
        public HttpResponseMessage PostLead(LeadDto leadDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var lead = leadDto.ToEntity();
            _leadRepository.Create(lead);
            _leadRepository.Flush();
            leadDto.LeadId = lead.Id;

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, leadDto);
            //response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = leadDto.LeadId }));
            return response;
        }

        // DELETE api/leads/lead/5
        public HttpResponseMessage DeleteLead(int id)
        {
            var lead = _leadRepository.Get(id);
            if (lead == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var leadDto = new LeadDto(lead);
            _leadRepository.Delete(lead);

            try
            {
                _leadRepository.Flush();
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, leadDto);
        }
    }
}