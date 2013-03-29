using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Leads.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.WebApi.Common;

namespace Coevery.Leads.Controllers
{
    public class LeadController : ContentController<Lead, LeadDto>
    {
        public LeadController(IOrchardServices services, IContentManager contentManager)
            : base(services, contentManager)
        {

        }
    }
}