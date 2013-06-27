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
    public class ClientRouteProvider : IClientRouteProvider {

        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder) {
            builder.Create("ProjectionList",
                           Feature,
                           route => route
                                        .Url("/Projections/{EntityName:[0-9a-zA-Z]+}")
                                        .TemplateUrl("'SystemAdmin/Projections/List'")
                                        .Controller("ProjectionListCtrl")
                                        .Dependencies("controllers/listcontroller"));

            builder.Create("ProjectionCreate",
                           Feature,
                           route => route
                                        .Url("/Projections/{EntityName:[0-9a-zA-Z]+}/Create")
                                        .TemplateUrl("function(params) { return 'SystemAdmin/Projections/Create/' + params.EntityName;}")
                                        .Controller("ProjectionDetailCtrl")
                                        .Dependencies("controllers/detailcontroller"));

            builder.Create("ProjectionEdit",
                           Feature,
                           route => route
                                        .Url("/Projections/{EntityName:[0-9a-zA-Z]+}/{Id:[0-9a-zA-Z]+}")
                                        .TemplateProvider(@"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Projections/Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]")
                                        .Controller("ProjectionDetailCtrl")
                                        .Dependencies("controllers/detailcontroller")
                );

        }
    }
}