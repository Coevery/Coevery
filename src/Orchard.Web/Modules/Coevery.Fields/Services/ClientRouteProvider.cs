using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Fields.Services {
    public class ClientRouteProvider : IClientRouteProvider {

        public void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("EntityDetail.Fields.Create")
                   .Configure(descriptor => {
                       descriptor.Url = "/Create";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Fields/CreateChooseType/' + params.Id; }";
                       descriptor.Controller = "FieldCreateChooseTypeCtrl";
                       descriptor.Dependencies = new string[] {"controllers/createchoosetypecontroller"};
                   });

            builder.Describe("EntityDetail.Fields.CreateEditInfo")
                   .Configure(descriptor => {
                       descriptor.Url = "/Create/{FieldTypeName:[0-9a-zA-Z]+}";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Fields/CreateEditInfo/' + params.Id + '?FieldTypeName=' + params.FieldTypeName; }";
                       //descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Fields/CreateEditInfo/' + params.FieldTypeName; }";
                       descriptor.Controller = "FieldCreateEditInfoCtrl";
                       descriptor.Dependencies = new string[] {"controllers/createeditinfocontroller"};
                   });

            builder.Describe("FieldEdit")
                   .Configure(descriptor => {
                       descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Edit/{FieldName:[0-9a-zA-Z]+}";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Fields/Edit/' + params.EntityName + '?FieldName=' + params.FieldName; }";
                       descriptor.Controller = "FieldEditCtrl";
                       descriptor.Dependencies = new string[] {"controllers/editcontroller"};
                   });
            builder.Describe("FieldItems")
                   .Configure(descriptor => {
                       descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Items/{FieldName:[0-9a-zA-Z]+}";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Fields/Items/' + params.EntityName; }";
                       descriptor.Controller = "ItemsCtrl";
                       descriptor.Dependencies = new string[] {"controllers/itemscontroller"};
                   });

            builder.Describe("FieldDependencyList")
                   .Configure(descriptor => {
                       descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Fields/DependencyList/' + params.EntityName; }";
                       descriptor.Controller = "FieldDependencyListCtrl";
                       descriptor.Dependencies = new string[] {"controllers/dependencylistcontroller"};
                   });

            builder.Describe("FieldDependencyCreate")
                   .Configure(descriptor => {
                       descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies/Create";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Fields/CreateDependency/' + params.EntityName; }";
                       descriptor.Controller = "FieldDependencyCreateCtrl";
                       descriptor.Dependencies = new string[] {"controllers/dependencycreatecontroller"};
                   });

            builder.Describe("FieldDependencyEdit")
                   .Configure(descriptor => {
                       descriptor.Url = "/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies/{DependencyID:[0-9]+}";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Fields/CreateDependency/' + params.EntityName + '?DependencyID=' + params.DependencyID; }";
                       descriptor.Controller = "FieldDependencyEditCtrl";
                       descriptor.Dependencies = new string[] {"controllers/dependencyeditcontroller"};
                   });

        }
    }
}