using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Opportunities.Models;
using Orchard.Data;

namespace Coevery.Opportunities.Controllers
{
    public class OppportunityController : ApiController
    {
        private IRepository<OpportunityRecord> _opportunityRepository;

        public OppportunityController(IRepository<OpportunityRecord> opportunityRepository)
        {
            _opportunityRepository = opportunityRepository;
        }

        // GET api/opportunities/opportunity
        public IEnumerable<OpportunityDto> GetOpportunities()
        {
            var reDtos = new List<OpportunityDto>();
            var re = _opportunityRepository.Table.ToList();
            reDtos.AddRange(re.Select(opportunity => new OpportunityDto(opportunity)));
            return reDtos;
        }

        // GET api/opportunities/opportunity/5
        public OpportunityDto GetOpportunity(int id)
        {
            var opportunity = _opportunityRepository.Get(id);
            if (opportunity == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return new OpportunityDto(opportunity);
        }

        // PUT api/opportunities/opportunity/5
        public HttpResponseMessage PutOpportunity(int id, OpportunityDto opportunityDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var oldopportunity = _opportunityRepository.Get(id);
            if (oldopportunity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var newopportunity = opportunityDto.ToEntity();
            _opportunityRepository.Copy(newopportunity, oldopportunity);

            try
            {
                _opportunityRepository.Flush();
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/opportunities/opportunity
        public HttpResponseMessage PostOpportunity(OpportunityDto opportunityDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var opportunity = opportunityDto.ToEntity();
            _opportunityRepository.Create(opportunity);
            _opportunityRepository.Flush();
            opportunityDto.OpportunityId = opportunity.Id;

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, opportunityDto);
            //response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = opportunityDto.opportunityId }));
            return response;
        }

        // DELETE api/opportunities/opportunity/5
        public HttpResponseMessage DeleteOpportunity(int id)
        {
            var opportunity = _opportunityRepository.Get(id);
            if (opportunity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var opportunityDto = new OpportunityDto(opportunity);
            _opportunityRepository.Delete(opportunity);

            try
            {
                _opportunityRepository.Flush();
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, opportunityDto);
        }
    }
}