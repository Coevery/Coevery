using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Models;
using Newtonsoft.Json.Linq;
using Orchard;

namespace Coevery.Core.Services
{
    public interface IClientRouteProvider:IDependency {
        void GetClientRoutes(ICollection<ClientRoute> clientRoutes);
        IEnumerable<ClientRoute> GetClientRoutes();
    }

    public class BaseClientRoute {
        public static IEnumerable<ClientRoute> GetClientRoutes(string stateModuleName,string moduleName)
        {
            yield return new ClientRoute
            {
                StateName = string.Format("{0}List", stateModuleName),
                ClientRouteInfo = new ClientRouteInfo
                {
                    definition = new Definition
                    {
                        url = string.Format("/{0}", moduleName),
                        templateUrl = new JRaw(string.Format("\"SystemAdmin/{0}/List\"", moduleName)),
                        controller = string.Format("{0}ListCtrl", stateModuleName),
                        dependencies = new[] { string.Format("Modules/Coevery.{0}/Scripts/controllers/listcontroller", moduleName) }
                    }
                }
            };

            yield return new ClientRoute
            {
                StateName = string.Format("{0}Create",stateModuleName),
                ClientRouteInfo = new ClientRouteInfo
                {
                    definition = new Definition
                    {
                        url = string.Format("/{0}/Create",moduleName),
                        templateUrl = new JRaw(string.Format("\"SystemAdmin/{0}/Create\"",moduleName)),
                        controller = string.Format("{0}EditCtrl",stateModuleName),
                        dependencies = new[] { string.Format("Modules/Coevery.{0}/Scripts/controllers/editcontroller",moduleName) }
                    }
                }
            };

            yield return new ClientRoute
            {
                StateName = string.Format("{0}Detail",stateModuleName),
                ClientRouteInfo = new ClientRouteInfo
                {
                    definition = new Definition
                    {
                        url = "/" + moduleName + "/{Id:[0-9a-zA-Z]+}",
                        templateUrl = new JRaw("function(params) { return 'SystemAdmin/" + moduleName + "/Edit/' + params.Id;}"),
                        controller = string.Format("{0}DetailCtrl",stateModuleName),
                        dependencies = new[] { string.Format("Modules/Coevery.{0}/Scripts/controllers/detailcontroller",moduleName) }
                    }
                }
            };
        }
    }
}