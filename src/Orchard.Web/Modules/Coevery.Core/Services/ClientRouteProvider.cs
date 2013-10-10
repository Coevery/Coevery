using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.Services {
    public class ClientRouteProvider : ClientRouteProviderBase {
        public ClientRouteProvider() {
            IsFrontEnd = true;
        }

        public override void Discover(ClientRouteTableBuilder builder) {
            var navigationView = new ClientViewDescriptor() {
                Name = "menulist",
                TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = '" + ModuleBasePath + @"ViewTemplate/MenuList';
                        return $http.get(url).then(function (response) { return response.data; });
                    }]",
                Controller = "NavigationCtrl"
            };
            navigationView.AddDependencies(ToAbsoluteScriptUrl, "controllers/navigationcontroller");

            builder.Describe("Root")
                .Configure(descriptor => {
                    descriptor.Url = "/";
                    descriptor.Views.Add(navigationView);
                });

            builder.Describe("Navigation")
                .Configure(descriptor => {
                    descriptor.Url = "/{NavigationId:[0-9]+}";
                    descriptor.Views.Add(navigationView);
                });

            builder.Describe("List")
                .Configure(descriptor => {
                    descriptor.Url = "/{NavigationId:[0-9]+}/{Module:[a-zA-Z]+}";
                    descriptor.Views.Add(navigationView);
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = '" + BasePath + @"' + $stateParams.Module + '/ViewTemplate/List/' + $stateParams.Module;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralListCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/listcontroller" });
                });

            builder.Describe("Create")
                .Configure(descriptor => {
                    descriptor.Url = "/{NavigationId:[0-9]+}/{Module:[a-zA-Z]+}/Create";
                    descriptor.Views.Add(navigationView);
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = '" + BasePath + @"' + $stateParams.Module + '/ViewTemplate/Create/' + $stateParams.Module;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralDetailCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/detailcontroller" });
                });

            builder.Describe("Detail")
                .Configure(descriptor => {
                    descriptor.Url = "/{NavigationId:[0-9]+}/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}";
                    descriptor.Views.Add(navigationView);
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = '" + BasePath + @"'+ $stateParams.Module + '/ViewTemplate/Edit/' + $stateParams.Id;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralDetailCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/detailcontroller" });
                });

            builder.Describe("View")
                .Configure(descriptor => {
                    descriptor.Url = "/{NavigationId:[0-9]+}/{Module:[a-zA-Z]+}/View/{Id:[0-9a-zA-Z]+}";
                    descriptor.Views.Add(navigationView);
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = '" + BasePath + @"' + $stateParams.Module + '/ViewTemplate/View/' + $stateParams.Id;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralViewCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/viewcontroller");
                });
        }
    }
}