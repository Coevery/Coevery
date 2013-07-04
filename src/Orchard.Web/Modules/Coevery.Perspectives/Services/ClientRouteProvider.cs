using Coevery.Core.ClientRoute;

namespace Coevery.Perspectives.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {
        public void Discover(ClientRouteTableBuilder builder)
        {
            builder.Describe("PerspectiveList")
                   .Configure(descriptor => {
                       descriptor.Url = "/Perspectives";
                       descriptor.TemplateUrl = "'SystemAdmin/Perspectives/List'";
                       descriptor.Controller = "PerspectiveListCtrl";
                       descriptor.Dependencies = new string[] {"controllers/listcontroller"};
                   });

            builder.Describe("PerspectiveCreate")
                   .Configure(descriptor => {
                       descriptor.Url = "/Perspectives/Create";
                       descriptor.TemplateUrl = "'SystemAdmin/Perspectives/Create'";
                       descriptor.Controller = "PerspectiveEditCtrl";
                       descriptor.Dependencies = new string[] {"controllers/editcontroller"};
                   });
            builder.Describe("PerspectiveEdit")
                   .Configure(descriptor => {
                       descriptor.Url = "/Perspectives/{Id:[0-9a-zA-Z]+}/Edit";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Perspectives/Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "PerspectiveEditCtrl";
                       descriptor.Dependencies = new string[] {"controllers/editcontroller"};
                   });

            builder.Describe("PerspectiveDetail")
                   .Configure(descriptor => {
                       descriptor.Url = "/Perspectives/{Id:[0-9a-zA-Z]+}";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Perspectives/Detail/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "PerspectiveDetailCtrl";
                       descriptor.Dependencies = new string[] {"controllers/detailcontroller"};
                   });

            builder.Describe("EditNavigationItem")
                   .Configure(descriptor => {
                       descriptor.Url = "/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/{NId:[0-9a-zA-Z]+}";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Perspectives/EditNavigationItem/' + $stateParams.NId; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "NavigationItemEditCtrl";
                       descriptor.Dependencies = new string[] {"controllers/navigationitemeditcontroller"};
                   });

            builder.Describe("CreateNavigationItem")
                   .Configure(descriptor => {
                       descriptor.Url = "/Perspectives/{Id:[0-9a-zA-Z]+}/Navigation/Create";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Perspectives/CreateNavigationItem/' + params.Id;}";
                       descriptor.Controller = "NavigationItemEditCtrl";
                       descriptor.Dependencies = new string[] {"controllers/navigationitemeditcontroller"};
                   });
        }
    }
}