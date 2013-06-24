using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Models;
using Newtonsoft.Json.Linq;
using Orchard;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.ClientRoute
{
    public interface IClientRouteProvider : IDependency {
        Feature Feature { get; }
        void Discover(ClientRouteBuilder builder);
    }

    public abstract class ClientRouteProviderBase : IClientRouteProvider {

        private readonly string _prefix;

        protected ClientRouteProviderBase(string prefix) {
            _prefix = prefix;
        }

        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder) {
            var displayPath = Feature.Descriptor.Extension.Path;
            builder.Create(_prefix + "List",
                           Feature,
                           route => route
                                        .Url(string.Format("/{0}", displayPath))
                                        .TemplateUrl(string.Format("'SystemAdmin/{0}/List'", displayPath))
                                        .Controller(_prefix + "ListCtrl")
                                        .Dependencies("controllers/listcontroller"));

            builder.Create(_prefix + "Create",
                           Feature,
                           route => route
                                        .Url("/" + displayPath + "/Create")
                                        .TemplateUrl("'SystemAdmin/" + displayPath + "/Create'")
                                        .Controller(_prefix + "EditCtrl")
                                        .Dependencies("controllers/editcontroller"));

            builder.Create(_prefix + "Detail",
                           Feature,
                           route => route
                                        .Url("/" + displayPath + "/{Id:[0-9a-zA-Z]+}")
                                        .Abstract(true)
                                        .TemplateUrl("function(params) { return 'SystemAdmin/" + displayPath + "/Detail/' + params.Id;}")
                                        .Controller(_prefix + "DetailCtrl")
                                        .Dependencies("controllers/detailcontroller"));
        }

    }

    //public class BaseClientRoute {
    //    public static IEnumerable<ClientRoute> GetClientRoutes(string stateModuleName,string moduleName)
    //    {
    //        yield return new ClientRoute
    //        {
    //            StateName = string.Format("{0}List", stateModuleName),
    //            ClientRouteInfo = new ClientRouteInfo
    //            {
    //                definition = new Definition
    //                {
    //                    url = string.Format("/{0}", moduleName),
    //                    templateUrl = new JRaw(string.Format("\"SystemAdmin/{0}/List\"", moduleName)),
    //                    controller = string.Format("{0}ListCtrl", stateModuleName),
    //                    dependencies = new[] { string.Format("Modules/Coevery.{0}/Scripts/controllers/listcontroller", moduleName) }
    //                }
    //            }
    //        };

    //        yield return new ClientRoute
    //        {
    //            StateName = string.Format("{0}Create",stateModuleName),
    //            ClientRouteInfo = new ClientRouteInfo
    //            {
    //                definition = new Definition
    //                {
    //                    url = string.Format("/{0}/Create",moduleName),
    //                    templateUrl = new JRaw(string.Format("\"SystemAdmin/{0}/Create\"",moduleName)),
    //                    controller = string.Format("{0}EditCtrl",stateModuleName),
    //                    dependencies = new[] { string.Format("Modules/Coevery.{0}/Scripts/controllers/editcontroller",moduleName) }
    //                }
    //            }
    //        };

    //        yield return new ClientRoute
    //        {
    //            StateName = string.Format("{0}Detail",stateModuleName),
    //            ClientRouteInfo = new ClientRouteInfo
    //            {
    //                definition = new Definition
    //                {
    //                    url = "/" + moduleName + "/{Id:[0-9a-zA-Z]+}",
    //                    templateUrl = new JRaw("function(params) { return 'SystemAdmin/" + moduleName + "/Edit/' + params.Id;}"),
    //                    controller = string.Format("{0}DetailCtrl",stateModuleName),
    //                    dependencies = new[] { string.Format("Modules/Coevery.{0}/Scripts/controllers/detailcontroller",moduleName) }
    //                }
    //            }
    //        };
    //    }
    //}
}