using System.Collections.Generic;
using System.Web.Http;
using Orchard;
using Orchard.ContentManagement;
using Orchard.WebApi.Common;

namespace Coevery.Core.Controllers
{
    public class CommonController :ApiController
    {
        // GET api/leads/lead
        public virtual IEnumerable<object> Get()
        {
            List<object> re = new List<object>();
           re.Add( new {Topic = "Topic",FirstName = "my",LastName = "you",StatusCode = "1"});

            return re;
        }
    }
}