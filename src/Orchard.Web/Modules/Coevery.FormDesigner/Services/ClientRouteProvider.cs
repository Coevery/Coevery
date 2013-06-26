using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.FormDesigner.Services {
    public class ClientRouteProvider : IClientRouteProvider {
        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder) {
            builder.Create("FormDesigner",
                          Feature,
                          route => route
                                       .Url("/FormDesigner/{EntityName:[0-9a-zA-Z]+}")
                                       .TemplateUrl("function(params) { return 'SystemAdmin/FormDesigner/Index/' + params.EntityName; }")
                                       .Controller("FormDesignerCtrl")
                                       .Dependencies("controllers/formdesignercontroller"));
        }
    }
}