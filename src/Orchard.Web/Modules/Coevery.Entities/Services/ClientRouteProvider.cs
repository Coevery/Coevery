using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Models;
using Coevery.Core.Services;
using Newtonsoft.Json.Linq;
using Orchard;

namespace Coevery.Entities.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {

        public void GetClientRoutes(ICollection<ClientRoute> clientRoutes)
        {
            foreach (var clientRoute in GetClientRoutes())
                clientRoutes.Add(clientRoute);
        }

        public IEnumerable<ClientRoute> GetClientRoutes()
        {
            yield return new ClientRoute
            {
                StateName = "EntityList",
                ClientRouteInfo = new ClientRouteInfo
                {
                    definition = new Definition
                    {
                        url = "/Entities",
                        templateUrl = new JRaw("\"SystemAdmin/Entities/List\""),
                        controller = "EntityListCtrl",
                        dependencies = new[] { "Modules/Coevery.Entities/Scripts/controllers/listcontroller" }
                    }
                }
            };

            yield return new ClientRoute
            {
                StateName = "EntityCreate",
                ClientRouteInfo = new ClientRouteInfo
                {
                    definition = new Definition
                    {
                        url = "/Entities/Create",
                        templateUrl = new JRaw("\"SystemAdmin/Entities/Create\""),
                        controller = "EntityEditCtrl",
                        dependencies = new[] { "Modules/Coevery.Entities/Scripts/controllers/editcontroller" }
                    }
                }
            };

            yield return new ClientRoute
            {
                StateName = "EntityDetail",
                ClientRouteInfo = new ClientRouteInfo
                {
                    definition = new Definition
                    {
                        url = "/Entities/{Id:[0-9a-zA-Z]+}",
                        @abstract = true,
                        templateUrl = new JRaw("function(params) { return 'SystemAdmin/Entities/Detail/' + params.Id;}"),
                        controller = "EntityDetailCtrl",
                        dependencies = new[] { "Modules/Coevery.Entities/Scripts/controllers/detailcontroller" }
                    },
                    children = new Dictionary<string, ClientRouteInfo>
                        {
                            {"Fields",new ClientRouteInfo() {
                                definition = new Definition
                                {
                                    url = string.Empty,
                                    templateUrl = new JRaw("\"SystemAdmin/Entities/Fields\""),
                                    controller = "FieldsCtrl",
                                    dependencies = new[] { "Modules/Coevery.Entities/Scripts/controllers/fieldscontroller"}
                                }
                            }}
                        },
                    Relationships = new Dictionary<string, Definition> {
                            {"definition",new Definition {
                                url = "/Relationships",
                                templateUrl = new JRaw("\"SystemAdmin/Entities/Relationships\""),
                                controller = "RelationshipsCtrl",
                                dependencies = new[] { "Modules/Coevery.Entities/Scripts/controllers/relationshipscontroller"}
                            }}
                        }
                }

            };
        }
    }
}