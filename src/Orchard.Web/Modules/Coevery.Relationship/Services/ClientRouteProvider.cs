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
                      descriptor.Controller = "EditOneToManyCtrl";
                      descriptor.Dependencies = new string[] { "controllers/onetomanydetailcontroller" };
                  });

            builder.Describe("CreateManyToMany")
                   .Configure(descriptor => {
                       descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/CreateManyToMany";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Relationship/CreateManyToMany/' + params.EntityName; }";
                       descriptor.Controller = "EditManyToManyCtrl";
                       descriptor.Dependencies = new string[] { "controllers/manytomanydetailcontroller" };
                   });
        }
    }
}