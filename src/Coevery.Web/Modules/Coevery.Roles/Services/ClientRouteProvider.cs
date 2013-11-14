using Coevery.Mvc.ClientRoute;

namespace Coevery.Roles.Services {
    public class ClientRouteProvider : ClientRouteProviderBase {
        public override void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("RoleList")
                .Configure(descriptor => { descriptor.Url = "/Roles"; })
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"List'";
                    view.Controller = "RoleListCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] {"controllers/listcontroller"});
                });

            builder.Describe("RoleCreate")
                .Configure(descriptor => { descriptor.Url = "/Roles/Create"; })
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Create'";
                    view.Controller = "RoleEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] {"controllers/editcontroller"});
                });

            builder.Describe("RoleEdit")
                .Configure(descriptor => { descriptor.Url = "/Roles/{Id:[0-9a-zA-Z]+}/Edit"; })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "RoleEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] {"controllers/editcontroller"});
                });
        }
    }
}