using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Relationship.Services {
    public class ClientRouteProvider : ClientRouteProviderBase {
        public override void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("EntityDetail.Relationships")
                .Configure(descriptor => {
                    descriptor.Url = "/Relationships";
                })
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Relationships'";
                    view.Controller = "RelationshipsCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] {"controllers/relationshipscontroller"});
                });

            builder.Describe("CreateOneToMany")
                .Configure(descriptor => {
                    descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/CreateOneToMany";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"CreateOneToMany/' + $stateParams.EntityName;
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "CreateOneToManyCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new [] {"controllers/onetomanydetailcontroller"});
                });

            builder.Describe("EditOneToMany")
                .Configure(descriptor => {
                    descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/EditOneToMany/{RelationId:[0-9]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"EditOneToMany/' + $stateParams.EntityName+ '?RelationId=' + $stateParams.RelationId;
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "EditOneToManyCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] {"controllers/editonetomanycontroller"});
                });

            builder.Describe("CreateManyToMany")
                .Configure(descriptor => {
                    descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/CreateManyToMany";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"CreateManyToMany/' + $stateParams.EntityName;
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "CreateManyToManyCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new [] {"controllers/manytomanydetailcontroller"});
                });

            builder.Describe("EditManyToMany")
                .Configure(descriptor => {
                    descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/EditManyToMany/{RelationId:[0-9]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"EditManyToMany/' + $stateParams.EntityName+ '?RelationId=' + $stateParams.RelationId;
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "EditManyToManyCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new [] {"controllers/editmanytomanycontroller"});
                });

            builder.Describe("View")
                .View(view => view.AddDependencies(ToAbsoluteScriptUrl, "controllers/relatedentitylistcontroller"));
        }
    }

    public class ClientFrontEndRouteProvider : ClientRouteProviderBase {

        public ClientFrontEndRouteProvider() {
            IsFrontEnd = true;
        }

        public override void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("View")
                .View(view => view.AddDependencies(ToAbsoluteScriptUrl, "controllers/relatedentitylistcontroller"));
        }
    }
}