using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.WebApi.Common;

namespace Coevery.Opportunities.Controllers
{
    public class OpportunitiesController : ContentController<Opportunity, OpportunityDto>
    {
        public OpportunitiesController(IOrchardServices services, IContentManager contentManager)
            : base(services, contentManager)
        {
        }


    }
}