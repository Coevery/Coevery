using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Localization;
using Orchard.Mvc.Filters;
using Orchard.Security;

namespace Coevery.AdminNavigation
{
    public class CoeveryAdminFilter : FilterProvider, IAuthorizationFilter
    {
        private readonly IAuthorizer _authorizer;

        public CoeveryAdminFilter(IAuthorizer authorizer)
        {
            _authorizer = authorizer;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (IsAdmin(filterContext))
            {
                if (!_authorizer.Authorize(StandardPermissions.AccessAdminPanel, T("Can't access the admin")))
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }

                Apply(filterContext.RequestContext);
            }
        }

        public static void Apply(RequestContext context)
        {
            // the value isn't important
            context.HttpContext.Items[typeof(CoeveryAdminFilter)] = null;
        }

        public static bool IsApplied(RequestContext context)
        {
            return context.HttpContext.Items.Contains(typeof(CoeveryAdminFilter));
        }

        private static bool IsAdmin(AuthorizationContext filterContext)
        {
            if (IsNameAdmin(filterContext) || IsNameAdminProxy(filterContext))
            {
                return true;
            }

            var adminAttributes = GetAdminAttributes(filterContext.ActionDescriptor);
            if (adminAttributes != null && adminAttributes.Any())
            {
                return true;
            }
            return false;
        }

        private static bool IsNameAdmin(AuthorizationContext filterContext)
        {
            return string.Equals(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "CoeveryAdmin",
                                 StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNameAdminProxy(AuthorizationContext filterContext)
        {
            return filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.StartsWith(
                "AdminControllerProxy", StringComparison.InvariantCultureIgnoreCase) &&
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Length == "CoeveryAdminControllerProxy".Length + 32;
        }

        private static IEnumerable<CoeveryAdminAttribute> GetAdminAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(CoeveryAdminAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(CoeveryAdminAttribute), true))
                .OfType<CoeveryAdminAttribute>();
        }
    }
}