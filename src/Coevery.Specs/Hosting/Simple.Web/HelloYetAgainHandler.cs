using System.Web;
using System.Web.Routing;

namespace Coevery.Specs.Hosting.Coevery.Web
{
    public class HelloYetAgainHandler : IRouteHandler, IHttpHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write("Hello yet again");
        }

        public bool IsReusable { get { return false; } }
    }
}