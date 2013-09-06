using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Localization;
using Orchard.Mvc.Filters;
using Orchard.Security;

namespace Coevery.Core.FrontMenu
{
    public class FrontMenuAuthorFilter : FilterProvider, IAuthorizationFilter
    {
        private readonly IAuthorizer _authorizer;

        public FrontMenuAuthorFilter(IAuthorizer authorizer)
        {
            _authorizer = authorizer;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (IsAdmin(filterContext))
            {
                Apply(filterContext.RequestContext);
            }
        }

        public static void Apply(RequestContext context)
        {
            // the value isn't important
            context.HttpContext.Items[typeof(FrontMenuAuthorFilter)] = null;
        }

        public static bool IsApplied(RequestContext context)
        {
            return context.HttpContext.Items.Contains(typeof(FrontMenuAuthorFilter));
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
            var result = string.Equals(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "Home",
                                 StringComparison.OrdinalIgnoreCase);
            if (!result) {
                result = string.Equals(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "Item",
                                 StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }

        private static bool IsNameAdminProxy(AuthorizationContext filterContext)
        {
            return filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.StartsWith(
                "HomeControllerProxy", StringComparison.InvariantCultureIgnoreCase) &&
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Length == "HomeControllerProxy".Length + 32;
        }

        private static IEnumerable<FrontMenuAttribute> GetAdminAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(FrontMenuAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(FrontMenuAttribute), true))
                .OfType<FrontMenuAttribute>();
        }
    }
}