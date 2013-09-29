using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Entities.Services {
    public class ClientRouteProvider : ClientRouteProviderBase {
        public override void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("EntityList")
                .Configure(descriptor => {
                    descriptor.Url = "/Entities";
                })
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"List'";
                    view.Controller = "EntityListCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/listcontroller");
                });

            builder.Describe("EntityCreate")
                .Configure(descriptor => {
                    descriptor.Url = "/Entities/Create";
                })
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Create'";
                    view.Controller = "EntityEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, "controllers/editcontroller");
                });

            builder.Describe("EntityEdit")
                .Configure(descriptor => {
                    descriptor.Url = "/Entities/{Id:[0-9a-zA-Z]+}/Edit";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "EntityEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/editcontroller" });
                });

            builder.Describe("EntityDetail")
                .Configure(descriptor => {
                    descriptor.Abstract = true;
                    descriptor.Url = "/Entities/{Id:[0-9a-zA-Z]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"Detail/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "EntityDetailCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/detailcontroller" });
                });

            builder.Describe("EntityDetail.Fields")
                .View(view => {
                    view.TemplateUrl = "'" + ModuleBasePath + @"Fields'";
                    view.Controller = "FieldsCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/fieldscontroller" });
                });

            #region Operate fields

            builder.Describe("EntityDetail.Fields.Create")
                .Configure(descriptor => {
                    descriptor.Url = "/Create";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return '" + ModuleBasePath + @"CreateChooseType/' + params.Id; }";
                    view.Controller = "FieldCreateChooseTypeCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/createchoosetypecontroller" });
                });

            builder.Describe("EntityDetail.Fields.CreateEditInfo")
                .Configure(descriptor => {
                    descriptor.Url = "/Create/{FieldTypeName:[0-9a-zA-Z]+}";
                    })
                .View(view => {
                    view.TemplateUrl = "function(params) { return '" + ModuleBasePath + @"CreateEditInfo/' + params.Id + '?FieldTypeName=' + params.FieldTypeName; }";
                    view.Controller = "FieldCreateEditInfoCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/createeditinfocontroller" });
                });

            builder.Describe("FieldEdit")
                .Configure(descriptor => {
                    descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Edit/{FieldName:[0-9a-zA-Z]+}";
                })
                .View(view => {
                    view.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = '" + ModuleBasePath + @"EditFields/' + $stateParams.EntityName + '?FieldName=' + $stateParams.FieldName;
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                    view.Controller = "FieldEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/editfieldscontroller" });
                });

            builder.Describe("FieldDependencyList")
                .Configure(descriptor => {
                    descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return '" + ModuleBasePath + @"DependencyList/' + params.EntityName; }";
                    view.Controller = "FieldDependencyListCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/dependencylistcontroller" });
                });

            builder.Describe("FieldDependencyCreate")
                .Configure(descriptor => {
                    descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies/Create";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return '" + ModuleBasePath + @"CreateDependency/' + params.EntityName; }";
                    view.Controller = "FieldDependencyCreateCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new[] { "controllers/dependencycreatecontroller" });
                });

            builder.Describe("FieldDependencyEdit")
                .Configure(descriptor => {
                    descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies/{DependencyID:[0-9]+}";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return '" + ModuleBasePath + @"EditDependency/' + params.EntityName + '?DependencyID=' + params.DependencyID; }";
                    view.Controller = "FieldDependencyEditCtrl";
                    view.AddDependencies(ToAbsoluteScriptUrl, new string[] { "controllers/dependencyeditcontroller" });
                });

            #endregion
        }
    }
}