using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Models;
using Coevery.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var resp = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
            return resp;
                            dependencies = new[] { "Modules/Coevery.Entities/Scripts/controllers/listcontroller" }
                        }
                    },
                    EntityDetail = new {
                        definition = new {
                            url = "/Entities/{Id:[0-9a-zA-Z]+}",
                            @abstract = true,
                            templateUrl = new JRaw("function(params) { return 'SystemAdmin/Entities/Detail/' + params.Id;}"),
                            controller = "EntityDetailCtrl",
                            dependencies = new[] { "Modules/Coevery.Entities/Scripts/controllers/detailcontroller" }
                        },
                        children = new {
                            Fields = new {
                                definition = new {
                                    url = "",
                                    templateUrl = "SystemAdmin/Entities/Fields",
                                    controller = "FieldsCtrl",
                                    dependencies = new[] { "Modules/Coevery.Entities/Scripts/controllers/fieldscontroller" }
                                }
                            },
                            Relationships = new {
                                definition = new {
                                    url = "/Relationships",
                                    templateUrl = "SystemAdmin/Entities/Relationships",
                                    controller = "RelationshipsCtrl",
                                    dependencies = new[] { "Modules/Coevery.Entities/Scripts/controllers/relationshipscontroller" }
                                }
                            }
        }

        private object GetRoutes()
        {
            Dictionary<string, ClientRouteInfo> tObjects = new Dictionary<string, ClientRouteInfo>();
            foreach (var routeProvider in _clientRouteProviders)
            {
                routeProvider.GetClientRoutes().ToList().ForEach(c => {
                    tObjects.Add(c.StateName,c.ClientRouteInfo);
                });
            }
            var routes = new
            {
                f = "/404",
                t = tObjects
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
