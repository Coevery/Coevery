using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.FormDesigner.Services {
    public class ClientRouteProvider : IClientRouteProvider {
        public void Discover(ClientRouteTableBuilder builder) {

            builder.Describe("FormDesigner")
                   .Configure(descriptor => {
                       descriptor.Url = "/FormDesigner/{EntityName:[0-9a-zA-Z]+}";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/FormDesigner/Index/' + $stateParams.EntityName; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "FormDesignerCtrl";
                       descriptor.Dependencies = new[] {"controllers/formdesignercontroller"};
                   });
        }
    }
}