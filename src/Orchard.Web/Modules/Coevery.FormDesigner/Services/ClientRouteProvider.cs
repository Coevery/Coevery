using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.FormDesigner.Services {
    public class ClientRouteProvider : IClientRouteProvider {
        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder) {
            builder.Create("FormDesigner",
                          Feature,
                          route => route
                                       .Url("/FormDesigner/{EntityName:[0-9a-zA-Z]+}")
                                       .TemplateProvider(@"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/FormDesigner/Index/' + $stateParams.EntityName; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]")
                                       .Controller("FormDesignerCtrl")
                                       .Dependencies("controllers/formdesignercontroller"));
        }
    }
}