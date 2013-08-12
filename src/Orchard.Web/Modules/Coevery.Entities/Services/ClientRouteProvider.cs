using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Entities.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {
        public void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("EntityList")
                   .Configure(descriptor => {
                       descriptor.Url = "/Entities";
                       descriptor.TemplateUrl = "'SystemAdmin/Entities/List'";
                       descriptor.Controller = "EntityListCtrl";
                       descriptor.Dependencies = new[] {"controllers/listcontroller"};
                   });

            builder.Describe("EntityCreate")
                   .Configure(descriptor => {
                       descriptor.Url = "/Entities/Create";
                       descriptor.TemplateUrl = "'SystemAdmin/Entities/Create'";
                       descriptor.Controller = "EntityEditCtrl";
                       descriptor.Dependencies = new[] {"controllers/editcontroller"};
                   });

            builder.Describe("EntityEdit")
                   .Configure(descriptor => {
                       descriptor.Url = "/Entities/{Id:[0-9a-zA-Z]+}/Edit";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Entities/Edit/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "EntityEditCtrl";
                       descriptor.Dependencies = new[] {"controllers/editcontroller"};
                   });

            builder.Describe("EntityDetail")
                   .Configure(descriptor => {
                       descriptor.Abstract = true;
                       descriptor.Url = "/Entities/{Id:[0-9a-zA-Z]+}";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Entities/Detail/' + $stateParams.Id; 
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "EntityDetailCtrl";
                       descriptor.Dependencies = new[] {"controllers/detailcontroller"};
                   });

            builder.Describe("EntityDetail.Fields")
                   .Configure(descriptor => {
                       descriptor.TemplateUrl = "'SystemAdmin/Entities/Fields'";
                       descriptor.Controller = "FieldsCtrl";
                       descriptor.Dependencies = new[] {"controllers/fieldscontroller"};
                   });

            builder.Describe("EntityDetail.Relationships")
                   .Configure(descriptor => {
                       descriptor.Url = "/Relationships";
                       descriptor.TemplateUrl = "'SystemAdmin/Entities/Relationships'";
                       descriptor.Controller = "RelationshipsCtrl";
                       descriptor.Dependencies = new[] { "controllers/relationshipscontroller" };
                   });

            #region -----------operate fields--------------

            builder.Describe("EntityDetail.Fields.Create")
                   .Configure(descriptor =>
                   {
                       descriptor.Url = "/Create";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Entities/CreateChooseType/' + params.Id; }";
                       descriptor.Controller = "FieldCreateChooseTypeCtrl";
                       descriptor.Dependencies = new string[] { "controllers/createchoosetypecontroller" };
                   });

            builder.Describe("EntityDetail.Fields.CreateEditInfo")
                   .Configure(descriptor =>
                   {
                       descriptor.Url = "/Create/{FieldTypeName:[0-9a-zA-Z]+}";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Entities/CreateEditInfo/' + params.Id + '?FieldTypeName=' + params.FieldTypeName; }";
                       descriptor.Controller = "FieldCreateEditInfoCtrl";
                       descriptor.Dependencies = new string[] { "controllers/createeditinfocontroller" };
                   });

            builder.Describe("FieldEdit")
                   .Configure(descriptor =>
                   {
                       descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Edit/{FieldName:[0-9a-zA-Z]+}";
                       descriptor.TemplateProvider = @"['$http', '$stateParams', function ($http, $stateParams) {
                                                var url = 'SystemAdmin/Entities/EditFields/' + $stateParams.EntityName + '?FieldName=' + $stateParams.FieldName;
                                                return $http.get(url).then(function(response) { return response.data; });
                                          }]";
                       descriptor.Controller = "FieldEditCtrl";
                       descriptor.Dependencies = new string[] { "controllers/editfieldscontroller" };
                   });

            builder.Describe("FieldEdit.Items")
                   .Configure(descriptor =>
                   {
                       descriptor.Url = "/Items";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Entities/Items/' + params.EntityName+ '?FieldName=' + params.FieldName; }";
                       descriptor.Controller = "ItemsCtrl";
                       descriptor.Dependencies = new string[] { "controllers/itemscontroller" };
                   });

            builder.Describe("FieldDependencyList")
                   .Configure(descriptor =>
                   {
                       descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Entities/DependencyList/' + params.EntityName; }";
                       descriptor.Controller = "FieldDependencyListCtrl";
                       descriptor.Dependencies = new string[] { "controllers/dependencylistcontroller" };
                   });

            builder.Describe("FieldDependencyCreate")
                   .Configure(descriptor =>
                   {
                       descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies/Create";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Entities/CreateDependency/' + params.EntityName; }";
                       descriptor.Controller = "FieldDependencyCreateCtrl";
                       descriptor.Dependencies = new string[] { "controllers/dependencycreatecontroller" };
                   });

            builder.Describe("FieldDependencyEdit")
                   .Configure(descriptor =>
                   {
                       descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies/{DependencyID:[0-9]+}";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Entities/EditDependency/' + params.EntityName + '?DependencyID=' + params.DependencyID; }";
                       descriptor.Controller = "FieldDependencyEditCtrl";
                       descriptor.Dependencies = new string[] { "controllers/dependencyeditcontroller" };
                   });

            #endregion
        }
    }
}