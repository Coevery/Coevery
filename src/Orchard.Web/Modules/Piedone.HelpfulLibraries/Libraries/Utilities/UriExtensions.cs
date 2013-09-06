using System;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Libraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class UriExtensions
    {
        /// <summary>
        /// Returns a protocol-relative URL, e.g. makes //orchardproject.net from http://orchardproject.net
        /// </summary>
        public static string ToStringWithoutScheme(this Uri uri)
        {
            if (!uri.IsAbsoluteUri) return uri.ToString();
            return "//" + uri.Host + uri.PathAndQuery;
        }
    }
}
