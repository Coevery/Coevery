using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;

namespace Contrib.ContentReference.Filters {
    [OrchardFeature("Contrib.ContextToken")]
    public class ContextFilter : FilterProvider, IActionFilter {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;

        public ContextFilter(IWorkContextAccessor workContextAccessor, IContentManager contentManager) {
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) {
            var area = (string)filterContext.RouteData.Values["area"];

            ContentItem item = null;

            if (area.Equals("Contents", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrEmpty((string)filterContext.RouteData.Values["Id"])) {
                int cid;
                if (int.TryParse((string) filterContext.RouteData.Values["Id"], out cid)) {
                    item = _contentManager.Get(cid);
                }
            }

            if (area.Equals("Orchard.Projections", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrEmpty((string)filterContext.RouteData.Values["id"])) {
                item = _contentManager.Get(int.Parse((string)filterContext.RouteData.Values["id"]));
            }

            if (item != null) {
                _workContextAccessor.GetContext().SetState("ContentContext", item);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {
        }
    }
}
