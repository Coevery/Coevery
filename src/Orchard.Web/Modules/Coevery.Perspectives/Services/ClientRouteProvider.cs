using Coevery.Core.ClientRoute;

namespace Coevery.Perspectives.Services
{
    public class ClientRouteProvider : ClientRouteProviderBase {
        public override void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("PerspectiveList")
                .Configure(descriptor => {
                    descriptor.Url = "/Perspectives";
                })
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"List'";
                    view.Controller = "PerspectiveListCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] { "controllers/listcontroller" });
                });

            builder.Describe("PerspectiveCreate")
                .Configure(descriptor => {
                    descriptor.Url = "/Perspectives/Create";
                })
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Create'";
                    view.Controller = "PerspectiveEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] { "controllers/editcontroller" });
                });

            builder.Describe("PerspectiveEdit")
                .Configure(descriptor => {
                    descriptor.Url = "/Perspectives/{Id:[0-9a-zA-Z]+}/Edit";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "PerspectiveEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] { "controllers/editcontroller" });
                });

            builder.Describe("PerspectiveDetail")
                .Configure(descriptor => {
                    descriptor.Url = "/Perspectives/{Id:[0-9a-zA-Z]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"Detail/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "PerspectiveDetailCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] { "controllers/detailcontroller" });
                });

            builder.Describe("CreateNavigationItem")
                .Configure(descriptor => {
                    descriptor.Url = "/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/Create";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return '" + ModuleBasePath + @"CreateNavigationItem/' + params.Id;}";
                    view.Controller = "NavigationItemEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] { "controllers/navigationitemeditcontroller" });
                });

            builder.Describe("EditNavigationItem")
                .Configure(descriptor => {
                    descriptor.Url = "/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/{NId:[0-9a-zA-Z]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"EditNavigationItem/' + $stateParams.NId; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "NavigationItemEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] { "controllers/navigationitemeditcontroller" });
                });
        }
    }
}