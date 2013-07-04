using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Metadata.Services {
    public class ClientRouteProvider : IClientRouteProvider {

        public void Discover(ClientRouteTableBuilder builder) {
            builder.Describe("EditOneToMany")
                   .Configure(descriptor => {
                       descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/EditOneToMany";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Metadata/EditOneToMany/' + params.EntityName; }";
                       descriptor.Controller = "EditOneToManyCtrl";
                       descriptor.Dependencies = new string[] {"controllers/onetomanydetailcontroller"};
                   });

            builder.Describe("EditManyToMany")
                   .Configure(descriptor => {
                       descriptor.Url = "/Relationships/{EntityName:[0-9a-zA-Z]+}/EditManyToMany";
                       descriptor.TemplateUrl = "function(params) { return 'SystemAdmin/Metadata/EditManyToMany/' + params.EntityName; }";
                       descriptor.Controller = "EditManyToManyCtrl";
                       descriptor.Dependencies = new string[] {"controllers/manytomanydetailcontroller"};
                   });
        }
    }
}