using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.ClientRoute;
using Coevery.Core.Models;
using Newtonsoft.Json;

namespace Coevery.Core.Controllers
{

    public class RouteController : ApiController
    {
        private readonly IEnumerable<IClientRouteProvider> _clientRouteProviders;

        public RouteController(IEnumerable<IClientRouteProvider> clientRouteProviders) {
            _clientRouteProviders = clientRouteProviders;
        }

        // GET api/route
        public HttpResponseMessage Get() {
            var routes = GetRoutes();

            var json = JsonConvert.SerializeObject(routes);
            var message = new HttpResponseMessage {Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")};
            return message;
        }

        private object GetRoutes() {
            var buidler = new ClientRouteBuilder();
            foreach (var routeProvider in _clientRouteProviders) {
                routeProvider.Discover(buidler);
            }

            var routes = new {
                f = "/404",
                t = buidler.Build()
            };
            return routes;
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
