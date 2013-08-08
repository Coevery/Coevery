using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Entities.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {
        public void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("EntityList")
                   .Configure(descriptor => {
                       descriptor.Url = "/Entities";
                       descriptor.TemplateUrl = "'SystemAdmin/Entities/List'";
                       descriptor.Controller = "EntityListCtrl";
                       descriptor.Dependencies = new[] {"controllers/listcontroller"};
                   });

            builder.Describe("EntityCreate")
                   .Configure(descriptor => {
                       descriptor.Url = "/Entities/Create";
                       descriptor.TemplateUrl = "'SystemAdmin/Entities/Create'";
                       descriptor.Controller = "EntityEditCtrl";
                       descriptor.Dependencies = new[] {"controllers/editcontroller"};
                   });

            builder.Describe("EntityEdit")
                   .Configure(descriptor => {
                       descriptor.Url = "/Entities/{Id:[0-9a-zA-Z]+}/Edit";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Entities/Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "EntityEditCtrl";
                       descriptor.Dependencies = new[] {"controllers/editcontroller"};
                   });

            builder.Describe("EntityDetail")
                   .Configure(descriptor => {
                       descriptor.Abstract = true;
                       descriptor.Url = "/Entities/{Id:[0-9a-zA-Z]+}";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Entities/Detail/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "EntityDetailCtrl";
                       descriptor.Dependencies = new[] {"controllers/detailcontroller"};
                   });

            builder.Describe("EntityDetail.Fields")
                   .Configure(descriptor => {
                       descriptor.TemplateUrl = "'SystemAdmin/Entities/Fields'";
                       descriptor.Controller = "FieldsCtrl";
                       descriptor.Dependencies = new[] {"controllers/fieldscontroller"};
                   });

            builder.Describe("EntityDetail.Relationships")
                   .Configure(descriptor => {
                       descriptor.Url = "/Relationships";
                       descriptor.TemplateUrl = "'SystemAdmin/Entities/Relationships'";
                       descriptor.Controller = "RelationshipsCtrl";
                       descriptor.Dependencies = new[] { "controllers/relationshipscontroller" };
                   });
        }
    }
}