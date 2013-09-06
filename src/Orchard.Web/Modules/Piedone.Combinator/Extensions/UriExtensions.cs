using System;

namespace Piedone.Combinator.Extensions
{
    // Use the one from Helpful Libraries once there will be more new things to use form it.
    public static class UriExtensions
    {
        public static string ToStringWithoutScheme(this Uri uri)
        {
            if (!uri.IsAbsoluteUri) return uri.ToString();
            return "//" + uri.Host + uri.PathAndQuery;
        }
    }
}