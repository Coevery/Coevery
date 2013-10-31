using System.Web.Routing;

namespace Coevery.Mvc {
    public interface IHasRequestContext {
        RequestContext RequestContext { get; }
    }
}