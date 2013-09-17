using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace Coevery.Projections.Controllers {
    public class ReferenceController : ApiController {
        public IEnumerable<JObject> Get(string id, string fieldName) {
            return null;
        }
    }
}