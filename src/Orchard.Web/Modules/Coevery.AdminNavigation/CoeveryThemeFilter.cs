using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Filters;
using Orchard.Themes;

namespace Coevery.AdminNavigation {
    public class CoeveryThemeFilter : FilterProvider, IActionFilter, IResultFilter {
        public void OnActionExecuting(ActionExecutingContext filterContext) {
            var attribute = GetThemedAttribute(filterContext.ActionDescriptor);
            if (CoeveryAdminFilter.IsApplied(filterContext.RequestContext)) {
                // admin are themed by default
                if (attribute == null || attribute.Enabled) {
                    Apply(filterContext.RequestContext);
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {}

        public void OnResultExecuting(ResultExecutingContext filterContext) {
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {}

        public static void Apply(RequestContext context) {
            // the value isn't important
            context.HttpContext.Items[typeof (ThemeFilter)] = null;
        }

        public static bool IsApplied(RequestContext context) {
            return context.HttpContext.Items.Contains(typeof (ThemeFilter));
        }

        private static ThemedAttribute GetThemedAttribute(ActionDescriptor descriptor) {
            return descriptor.GetCustomAttributes(typeof (ThemedAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof (ThemedAttribute), true))
                .OfType<ThemedAttribute>()
                .FirstOrDefault();
        }
    }
}