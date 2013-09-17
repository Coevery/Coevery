using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Core.Services {
    public class ClientRouteProvider : ClientRouteProviderBase {
        public ClientRouteProvider() {
            IsFrontEnd = true;
        }

        public override void Discover(ClientRouteTableBuilder builder) {
            ClientViewDescriptor navview = new ClientViewDescriptor()
            {
                Name = "menulist",
                TemplateProvider = @"['$http','$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/CoeveryCore/ViewTemplate/MenuList/$stateParams.Navigation';
                        return $http.get(url).then(function (response) { return response.data; });
                    }]",
                Controller = "MenuListCtrl",
                Dependencies = ToClientUrl(new[] { "controllers/menulistcontroller" })
            };

            //builder.Describe("Root")
            //    .Configure(descriptor =>
            //    {
            //        descriptor.Url = "";
            //        descriptor.Views.Add(navview);
            //    });

            builder.Describe("Navigation")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/{Navigation:[a-zA-Z0-9]+}";
                    descriptor.Views.Add(navview);
                });

            builder.Describe("List")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/{Navigation:[a-zA-Z0-9]+}/{Module:[a-zA-Z]+}";
                    descriptor.Views.Add(navview);
                })
                .View(view =>
                {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/List/' + $stateParams.Module;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralListCtrl";
                    view.Dependencies = ToClientUrl(new[] { "controllers/listcontroller" });
                });

            builder.Describe("Create")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/{Navigation:[a-zA-Z0-9]+}/{Module:[a-zA-Z]+}/Create";
                    descriptor.Views.Add(navview);
                })
                .View(view =>
                {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/Create/' + $stateParams.Module;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralDetailCtrl";
                    view.Dependencies = ToClientUrl(new[] { "controllers/detailcontroller" });
                });

            builder.Describe("Detail")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/{Navigation:[a-zA-Z0-9]+}/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}";
                    descriptor.Views.Add(navview);
                })
                .View(view =>
                {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/Edit/' + $stateParams.Id;
                        return $http.get(url).then(function (response) { return response.data; });
                    }]";
                    view.Controller = "GeneralDetailCtrl";
                    view.Dependencies = ToClientUrl(new[] { "controllers/detailcontroller" });
                });

            builder.Describe("View")
                .Configure(descriptor =>
                {
                    descriptor.Url = "/{Navigation:[a-zA-Z0-9]+}/{Module:[a-zA-Z]+}/View/{Id:[0-9a-zA-Z]+}";
                    descriptor.Views.Add(navview);
                })
                .View(view =>
                {
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