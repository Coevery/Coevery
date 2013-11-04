using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Coevery.Environment;
using Coevery.WarmupStarter;

namespace Coevery.Web {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication {
        private static Starter<ICoeveryHost> _starter;

        public MvcApplication() {
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }

        protected void Application_Start() {
            RegisterRoutes(RouteTable.Routes);
            _starter = new Starter<ICoeveryHost>(HostInitialization, HostBeginRequest, HostEndRequest);
            _starter.OnApplicationStart(this);
        }

        protected void Application_BeginRequest() {
            _starter.OnBeginRequest(this);
        }

        protected void Application_EndRequest() {
            _starter.OnEndRequest(this);
        }

        private static void HostBeginRequest(HttpApplication application, ICoeveryHost host) {
            application.Context.Items["originalHttpContext"] = application.Context;
            host.BeginRequest();
        }

        private static void HostEndRequest(HttpApplication application, ICoeveryHost host) {
            host.EndRequest();
        }

        private static ICoeveryHost HostInitialization(HttpApplication application) {
            var host = CoeveryStarter.CreateHost(MvcSingletons);

            host.Initialize();

            // initialize shells to speed up the first dynamic query
            host.BeginRequest();
            host.EndRequest();

            return host;
        }

        static void MvcSingletons(ContainerBuilder builder) {
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();
        }
    }
}
