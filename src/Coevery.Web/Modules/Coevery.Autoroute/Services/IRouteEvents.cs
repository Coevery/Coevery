using System;
using Coevery.ContentManagement;
using Coevery.Events;

namespace Coevery.Autoroute.Services {
    public interface IRouteEvents : IEventHandler {
        void Routed(IContent content, String path);
    }
}