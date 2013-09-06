using System;
using System.IO;
using Orchard.UI.Resources;

namespace Piedone.Combinator.Extensions
{
    public static class ResourceDefinitionExtensions
    {
        /// <summary>
        /// Gets the ultimate full path of a resource, even if it uses CDN. Note that the paths are not uniform, they're
        /// concatenated from the resoure's paths, therefore they can well be virtual relative paths (starting with a tilde)
        /// or relative public urls.
        /// </summary>
        public static string GetFullPath(this ResourceDefinition resource)
        {
            if (String.IsNullOrEmpty(resource.Url)) return resource.UrlCdn;

            if (resource.Url.Contains("~")) return resource.Url;

            return Path.Combine(resource.BasePath + resource.Url);
        }

        public static void SetUrlWithoutScheme(this ResourceDefinition resource, Uri url)
        {
            resource.SetUrl(url.ToStringWithoutScheme());
        }
    }
}