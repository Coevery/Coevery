using Coevery.Core.ClientRoute;
using Orchard.Environment.Extensions.Models;

namespace Coevery.FormDesigner.Services {
    public class ClientRouteProvider : IClientRouteProvider {
        public virtual Feature Feature { get; set; }

        public void Discover(ClientRouteBuilder builder) {
            
        }
    }
}