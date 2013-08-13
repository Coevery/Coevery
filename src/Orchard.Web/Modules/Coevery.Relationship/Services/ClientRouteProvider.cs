using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Relationship.Services
{
    public class ClientRouteProvider : IClientRouteProvider
    {
        public void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("EntityDetail.Relationships")
                   .Configure(descriptor => {
                       descriptor.Url = "/Relationships";
                       descriptor.TemplateUrl = "'SystemAdmin/Relationship/Relationships'";
                       descriptor.Controller = "RelationshipsCtrl";
                       descriptor.Dependencies = new[] { "controllers/relationshipscontroller" };
                   });

            builder.Describe("CreateOneToMany")
                  .Configure(descriptor => {
                      descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/CreateOneToMany";
                      descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Relationship/CreateOneToMany/' + params.EntityName; }";
                      descriptor.Controller = "CreateOneToManyCtrl";
                      descriptor.Dependencies = new string[] { "controllers/onetomanydetailcontroller" };
                  });

            builder.Describe("EditOneToMany")
                  .Configure(descriptor => {
                      descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/EditOneToMany/{RelationId:[0-9]+}";
                      descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Relationship/EditOneToMany/'" +
                          " + params.EntityName + '?RelationId=' + params.RelationId; }";
                      descriptor.Controller = "EditOneToManyCtrl";
                      descriptor.Dependencies = new string[] { "controllers/editonetomanycontroller" };
                  });

            builder.Describe("CreateManyToMany")
                   .Configure(descriptor => {
                       descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/CreateManyToMany";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Relationship/CreateManyToMany/' + params.EntityName; }";
                       descriptor.Controller = "CreateManyToManyCtrl";
                       descriptor.Dependencies = new string[] { "controllers/manytomanydetailcontroller" };
                   });

            builder.Describe("EditManyToMany")
                   .Configure(descriptor => {
                       descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/EditManyToMany/{RelationId:[0-9]+}";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Relationship/EditManyToMany/'" +
                       " + params.EntityName + '?RelationId=' + params.RelationId; }";
                       descriptor.Controller = "EditManyToManyCtrl";
                       descriptor.Dependencies = new string[] { "controllers/editmanytomanycontroller" };
                   });
        }
    }
}