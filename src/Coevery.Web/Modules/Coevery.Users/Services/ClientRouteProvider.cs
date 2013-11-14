using Coevery.Mvc.ClientRoute;

namespace Coevery.Users.Services {
    public class ClientRouteProvider : ClientRouteProviderBase {
        public override void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("UserList")
                .Configure(descriptor => { descriptor.Url = "/Users"; })
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"List'";
                    view.Controller = "UserListCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] {"controllers/listcontroller"});
                });

            builder.Describe("UserCreate")
                .Configure(descriptor => { descriptor.Url = "/Users/Create"; })
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Create'";
                    view.Controller = "UserEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] {"controllers/editcontroller"});
                });

            builder.Describe("UserEdit")
                .Configure(descriptor => { descriptor.Url = "/Users/{Id:[0-9a-zA-Z]+}/Edit"; })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "UserEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] {"controllers/editcontroller"});
                });
        }
    }
}