using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.ClientRoute;
using Coevery.Core.Models;
using Coevery.Core.Services;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Projections.Services
{
    public class ClientRouteProvider : ClientRouteProviderBase {
        public ClientRouteProvider() : base("Projection") {}
    }
}