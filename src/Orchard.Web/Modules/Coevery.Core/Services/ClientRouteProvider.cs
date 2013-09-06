using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.Services {
    public class ClientRouteProvider : ClientRouteProviderBase {
        public ClientRouteProvider() {
            IsFrontEnd = true;
        }

        public override void Discover(ClientRouteTableBuilder builder) {
            
            builder.Describe("List")
                .Configure(descriptor => {
                    descriptor.Url = "/{Module:[a-zA-Z]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/List/' + $stateParams.Module;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralListCtrl";
                    view.Dependencies = ToClientUrl(new[] { "controllers/listcontroller" });
                });

            builder.Describe("Create")
                .Configure(descriptor => {
                    descriptor.Url = "/{Module:[a-zA-Z]+}/Create";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/Create/' + $stateParams.Module;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralDetailCtrl";
                    view.Dependencies = ToClientUrl(new[] { "controllers/detailcontroller" });
                });

            builder.Describe("Detail")
                .Configure(descriptor => {
                    descriptor.Url = "/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/Edit/' + $stateParams.Id;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralDetailCtrl";
                    view.Dependencies = ToClientUrl(new[] { "controllers/detailcontroller" });
                });

            builder.Describe("View")
                .Configure(descriptor => {
                    descriptor.Url = "/{Module:[a-zA-Z]+}/View/{Id:[0-9a-zA-Z]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/View/' + $stateParams.Id;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralViewCtrl";
                    view.Dependencies = ToClientUrl(new[] { "controllers/viewcontroller", "controllers/relatedentitylistcontroller" });
                });
        }
    }
}