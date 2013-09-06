using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.Mvc.ViewEngines.ThemeAwareness;

namespace Coevery.Core.Services
{
    public interface ITemplateViewService : IDependency
    {
        string RenderView(string area, string controllerName, string viewName);
        string RenderView(string area, string controllerName, string viewName, ViewDataDictionary data);
    }

    public class TemplateViewService : ITemplateViewService
    {
        private readonly ILayoutAwareViewEngine _layoutAwareViewEngine;
        private readonly IWorkContextAccessor _workContextAccessor;
        public TemplateViewService(ILayoutAwareViewEngine layoutAwareViewEngine,
            IWorkContextAccessor workContextAccessor)
        {
            _layoutAwareViewEngine = layoutAwareViewEngine;
            _workContextAccessor = workContextAccessor;
        }

        private class EmptyController : ControllerBase
        {
            protected override void ExecuteCore() { }
        }

        public string RenderView(string area, string controllerName, string viewName)
        {
            return RenderView(area, controllerName, viewName, new ViewDataDictionary());
        }

        public string RenderView(string area, string controllerName, string viewName, ViewDataDictionary data)
        {
            HttpContextBase contextBase = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);
            routeData.Values.Add("area", area);
            routeData.DataTokens.Add("area", area);
            routeData.DataTokens.Add("IWorkContextAccessor", _workContextAccessor);

            var controllerContext = new ControllerContext(contextBase, routeData, new EmptyController());

            var razorViewResult = _layoutAwareViewEngine.FindView(controllerContext, viewName, "", false);

            var writer = new StringWriter();
            var viewContext = new ViewContext(controllerContext, razorViewResult.View, data, new TempDataDictionary(), writer);
            razorViewResult.View.Render(viewContext, writer);

            return writer.ToString();
        }
    }
}
