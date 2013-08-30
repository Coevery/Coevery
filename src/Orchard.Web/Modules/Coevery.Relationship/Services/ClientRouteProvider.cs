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
                    view.TemplateUrl = "'SystemAdmin/Relationship/Relationships'";
                    view.Controller = "RelationshipsCtrl";
                    view.Dependencies = ToClientUrl(new[] {"controllers/relationshipscontroller"});
                });

            builder.Describe("CreateOneToMany")
                .Configure(descriptor => {
                    descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/CreateOneToMany";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return 'SystemAdmin/Relationship/CreateOneToMany/' + params.EntityName; }";
                    view.Controller = "CreateOneToManyCtrl";
                    view.Dependencies = ToClientUrl(new string[] {"controllers/onetomanydetailcontroller"});
                });

            builder.Describe("EditOneToMany")
                .Configure(descriptor => {
                    descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/EditOneToMany/{RelationId:[0-9]+}";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return 'SystemAdmin/Relationship/EditOneToMany/'" +
                                       " + params.EntityName + '?RelationId=' + params.RelationId; }";
                    view.Controller = "EditOneToManyCtrl";
                    view.Dependencies = ToClientUrl(new string[] {"controllers/editonetomanycontroller"});
                });

            builder.Describe("CreateManyToMany")
                .Configure(descriptor => {
                    descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/CreateManyToMany";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return 'SystemAdmin/Relationship/CreateManyToMany/' + params.EntityName; }";
                    view.Controller = "CreateManyToManyCtrl";
                    view.Dependencies = ToClientUrl(new string[] {"controllers/manytomanydetailcontroller"});
                });

            builder.Describe("EditManyToMany")
                .Configure(descriptor => {
                    descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/EditManyToMany/{RelationId:[0-9]+}";
                })
                .View(view => {
                    view.TemplateUrl = "function(params) { return 'SystemAdmin/Relationship/EditManyToMany/'" +
                                       " + params.EntityName + '?RelationId=' + params.RelationId; }";
                    view.Controller = "EditManyToManyCtrl";
                    view.Dependencies = ToClientUrl(new string[] {"controllers/editmanytomanycontroller"});
                });
        }
    }
}