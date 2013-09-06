using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Localization;
using Orchard.Mvc.Filters;
using Orchard.Security;

namespace Coevery.Core.Admin {
    public class SystemAdminFilter : FilterProvider, IAuthorizationFilter {
        private readonly IAuthorizer _authorizer;

        public SystemAdminFilter(IAuthorizer authorizer) {
            _authorizer = authorizer;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext) {
            if (IsAdmin(filterContext)) {
                if (!_authorizer.Authorize(StandardPermissions.AccessAdminPanel, T("Can't access the admin"))) {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
                Apply(filterContext.RequestContext);
            }
        }

        public static void Apply(RequestContext context) {
            // the value isn't important
            context.HttpContext.Items[typeof(SystemAdminFilter)] = null;
        }

        public static bool IsApplied(RequestContext context) {
            return context.HttpContext.Items.Contains(typeof(SystemAdminFilter));
        }

        private static bool IsAdmin(AuthorizationContext filterContext) {
            if (IsNameAdmin(filterContext) || IsNameAdminProxy(filterContext)) {
                return true;
            }

            var adminAttributes = GetAdminAttributes(filterContext.ActionDescriptor);
            if (adminAttributes != null && adminAttributes.Any()) {
                return true;
            }
            return false;
        }

        private static bool IsNameAdmin(AuthorizationContext filterContext) {
            return string.Equals(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "SystemAdmin",
                                 StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNameAdminProxy(AuthorizationContext filterContext) {
            return filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.StartsWith(
                "SystemAdminControllerProxy", StringComparison.InvariantCultureIgnoreCase) &&
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Length == "SystemAdminControllerProxy".Length + 32;
        }

        private static IEnumerable<SystemAdminAttribute> GetAdminAttributes(ActionDescriptor descriptor) {
            return descriptor.GetCustomAttributes(typeof(SystemAdminAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(SystemAdminAttribute), true))
                .OfType<SystemAdminAttribute>();
        }
    }
}