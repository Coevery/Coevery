using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Piedone.HelpfulLibraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class AuthenticationServiceExtensions
    {
        /// <summary>
        /// Checks if the current user is authenticated or not
        /// </summary>
        public static bool IsAuthenticated(this IAuthenticationService service)
        {
            return service.GetAuthenticatedUser() != null;
        }
    }
}
