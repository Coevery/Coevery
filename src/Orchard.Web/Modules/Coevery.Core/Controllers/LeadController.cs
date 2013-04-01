using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Models;
using DynamicTypes.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.WebApi.Common;

namespace CoeveryCore.Controllers
{
    public class LeadsController : ContentController<Lead, LeadDto>
    {
        public LeadsController(IOrchardServices services, IContentManager contentManager)
            : base(services, contentManager)
        {

        }
    }
}