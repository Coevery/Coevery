using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.FormDesigner.Services {
    public class ClientRouteProvider : ClientRouteProviderBase {
        public override void Discover(ClientRouteTableBuilder builder) {

            builder.Describe("FormDesigner")
                .Configure(descriptor => {
                    descriptor.Url = "/FormDesigner/{EntityName:[0-9a-zA-Z]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"Index/' + $stateParams.EntityName; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "FormDesignerCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/formdesignercontroller" });
                });
        }
    }
}