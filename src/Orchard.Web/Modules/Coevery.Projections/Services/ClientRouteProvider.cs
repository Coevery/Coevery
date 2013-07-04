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

        public void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("ProjectionList")
                   .Configure(descriptor => {
                       descriptor.Url = "/Projections/{EntityName:[0-9a-zA-Z]+}";
                       descriptor.TemplateUrl = "'SystemAdmin/Projections/List'";
                       descriptor.Controller = "ProjectionListCtrl";
                       descriptor.Dependencies = new[] {"controllers/listcontroller"};
                   });

            builder.Describe("ProjectionCreate")
                   .Configure(descriptor => {
                       descriptor.Url = "/Projections/{EntityName:[0-9a-zA-Z]+}/Create";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Projections/Create/' + params.EntityName;}";
                       descriptor.Controller = "ProjectionDetailCtrl";
                       descriptor.Dependencies = new[] {"controllers/detailcontroller"};
                   });

            builder.Describe("ProjectionEdit")
                   .Configure(descriptor => {
                       descriptor.Url = "/Projections/{EntityName:[0-9a-zA-Z]+}/{Id:[0-9a-zA-Z]+}";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Projections/Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "ProjectionDetailCtrl";
                       descriptor.Dependencies = new[] {"controllers/detailcontroller"};
                   });
        }
    }
}