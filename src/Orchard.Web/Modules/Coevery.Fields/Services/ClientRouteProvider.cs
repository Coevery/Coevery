using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Fields.Services {
    public class ClientRouteProvider : IClientRouteProvider {
        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder) {
            builder.Create("FieldCreateChooseType",
                           Feature,
                           route => route
                                        .Url("/Fields/{EntityName:[0-9a-zA-Z]+}/Create")
                                        .TemplateUrl("function(params) { return 'SystemAdmin/Fields/CreateChooseType/' + params.EntityName; }")
                                        .Controller("FieldCreateChooseTypeCtrl")
                                        .Dependencies("controllers/createchoosetypecontroller"));

            builder.Create("FieldCreateEditInfo",
                           Feature,
                           route => route
                                        .Url("/Fields/{EntityName:[0-9a-zA-Z]+}/Create/{FieldTypeName:[0-9a-zA-Z]+}")
                                        .TemplateUrl("function(params) { return 'SystemAdmin/Fields/CreateEditInfo/' + params.EntityName + '?FieldTypeName=' + params.FieldTypeName; }")
                                        .Controller("FieldCreateEditInfoCtrl")
                                        .Dependencies("controllers/createeditinfocontroller"));

            builder.Create("FieldEdit",
                          Feature,
                          route => route
                                       .Url("/Fields/{EntityName:[0-9a-zA-Z]+}/Edit/{FieldName:[0-9a-zA-Z]+}")
                                       .TemplateUrl("function(params) { return 'SystemAdmin/Fields/Edit/' + params.EntityName + '?FieldName=' + params.FieldName; }")
                                       .Controller("FieldEditCtrl")
                                       .Dependencies("controllers/editcontroller"));

            builder.Create("FieldItems",
                         Feature,
                         route => route
                                      .Url("/Fields/{EntityName:[0-9a-zA-Z]+}/Items/{FieldName:[0-9a-zA-Z]+}")
                                      .TemplateUrl("function(params) { return 'SystemAdmin/Fields/Items/' + params.EntityName; }")
                                      .Controller("ItemsCtrl")
                                      .Dependencies("controllers/itemscontroller"));

            builder.Create("FieldDependencyList",
                      Feature,
                      route => route
                                   .Url("/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies")
                                   .TemplateUrl("function(params) { return 'SystemAdmin/Fields/DependencyList/' + params.EntityName; }")
                                   .Controller("FieldDependencyListCtrl")
                                   .Dependencies("controllers/dependencylistcontroller"));

            builder.Create("FieldDependencyCreate",
                    Feature,
                    route => route
                                 .Url("/Fields/{EntityName:[0-9a-zA-Z]+}/Dependencies/Create")
                                 .TemplateUrl("function(params) { return 'SystemAdmin/Fields/CreateDependency/' + params.EntityName; }")
                                 .Controller("FieldDependencyCreateCtrl")
                                 .Dependencies("controllers/dependencycreatecontroller"));
        }
    }
}