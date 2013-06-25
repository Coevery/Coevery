using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.Metadata.Services {
    public class ClientRouteProvider : IClientRouteProvider {
        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder) {
            builder.Create("EditOneToMany",
                           Feature,
                           route => route
                                        .Url("/Relationships/{EntityName:[0-9a-zA-Z]+}/EditOneToMany")
                                        .TemplateUrl("function(params) { return 'SystemAdmin/Metadata/EditOneToMany/' + params.EntityName; }")
                                        .Controller("EditOneToManyCtrl")
                                        .Dependencies("controllers/onetomanydetailcontroller"));

            builder.Create("EditManyToMany",
                           Feature,
                           route => route
                                        .Url("/Relationships/{EntityName:[0-9a-zA-Z]+}/EditManyToMany")
                                        .TemplateUrl("function(params) { return 'SystemAdmin/Metadata/EditManyToMany/' + params.EntityName; }")
                                        .Controller("EditManyToManyCtrl")
                                        .Dependencies("controllers/manytomanydetailcontroller"));
        }
    }
}