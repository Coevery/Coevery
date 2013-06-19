using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coevery.Core.Controllers
{
    public class RouteController : ApiController
    {
        // GET api/route
        public HttpResponseMessage Get() {

            var route = new {
                f = "/404",
                t = new {
                    List = new {
                        d = new {
                            url = "/Entities",
                            templateUrl = "SystemAdmin/Entities/List",
                            controller = "EntityListCtrl",
                            dependencies = new[] {"Modules/Coevery.Entities/Scripts/controllers/listcontroller"}
                        }
                    }
                }
            };


            var json = JsonConvert.SerializeObject(route);

            var resp = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
            return resp;
        }

        // GET api/route/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/route
        public void Post([FromBody]string value)
        {
        }

        // PUT api/route/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/route/5
        public void Delete(int id)
        {
        }
    }
}
