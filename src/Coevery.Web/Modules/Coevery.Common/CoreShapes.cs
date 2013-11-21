using System.Collections.Generic;
using System.IO;
using Coevery.DisplayManagement;
using Coevery.DisplayManagement.Descriptors;
using Coevery.Mvc;
using Coevery.Mvc.ClientRoute;
using Coevery.UI.Navigation;

namespace Coevery.Common {
    public class CoreShapes : IShapeTableProvider {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClientRouteTableManager _clientRouteTableManager;

        public CoreShapes(IHttpContextAccessor httpContextAccessor,
            IClientRouteTableManager clientRouteTableManager) {
            _httpContextAccessor = httpContextAccessor;
            _clientRouteTableManager = clientRouteTableManager;
        }

        [Shape]
        public void ClientRoute(dynamic Display, dynamic Shape, TextWriter Output) {
            var isFrontEnd = Shape.IsFrontEnd;
            var routes = _clientRouteTableManager.GetRouteTable(isFrontEnd);
            var result = Display.ClientBootstrapScript(IsFrontEnd: isFrontEnd, Routes: routes);
            Output.Write(result);
        }

        [Shape]
        public void PerspectiveMenuList(dynamic Display, IEnumerable<MenuItem> Shape, TextWriter Output) {
            
        }

        [Shape]
        public void NavigationMenuItem(dynamic Display, MenuItem Shape, TextWriter Output) {

        }

        public void Discover(ShapeTableBuilder builder)
        {
            
        }
    }
}