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
    public class ClientRouteProvider : ClientRouteProviderBase {

        public override void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("EntityDetail.Views")
                .Configure(descriptor => {
                    descriptor.Url = "/Views";
                })
                .View(view => {
                    view.TemplateUrl = "'SystemAdmin/Projections/List'";
                    view.Controller = "ProjectionListCtrl";
                    view.AddDependencies(ToClientUrl, new[] { "controllers/listcontroller" });
                });

            builder.Describe("ProjectionCreate")
                .Configure(descriptor => {
                    descriptor.Url = "/Projections/{EntityName:[0-9a-zA-Z]+}/Create";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return 'SystemAdmin/Projections/Create/' + params.EntityName;}";
                    view.Controller = "ProjectionDetailCtrl";
                    view.AddDependencies(ToClientUrl, new[] { "controllers/detailcontroller" });
                });

            builder.Describe("ProjectionEdit")
                .Configure(descriptor => {
                    descriptor.Url = "/Projections/{EntityName:[0-9a-zA-Z]+}/{Id:[0-9a-zA-Z]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Projections/Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "ProjectionDetailCtrl";
                    view.AddDependencies(ToClientUrl, new[] { "controllers/detailcontroller" });
                });
        }
    }
}