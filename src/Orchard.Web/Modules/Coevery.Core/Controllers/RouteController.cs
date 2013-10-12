using System.Net.Http;
using System.Web.Http;
using Coevery.Core.ClientRoute;
using Newtonsoft.Json;

namespace Coevery.Core.Controllers {
    public class RouteController : ApiController {
        private readonly IClientRouteTableManager _clientRouteTableManager;

        public RouteController(IClientRouteTableManager clientRouteTableManager) {
            _clientRouteTableManager = clientRouteTableManager;
        }

        // GET api/route
        public HttpResponseMessage Get(bool isFront) {
            var routes = new {
                f = "/404",
                t = _clientRouteTableManager.GetRouteTable(isFront)
            };

            var json = JsonConvert.SerializeObject(routes);
            var message = new HttpResponseMessage {Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")};
            return message;
        }
    }
}