using System.Collections.Generic;
using System.Linq;
using Orchard.UI.Resources;

namespace Piedone.Combinator.Extensions
{
    public static class ResourceListExtensions
    {
        public static int GetResourceListHashCode<T>(this IList<T> resources) where T: ResourceRequiredContext
        {
            var key = "";

            resources.ToList().ForEach(resource => key += resource.Resource.GetFullPath() + "__");

            return key.GetHashCode();
        }

        public static IList<T> SetLocation<T>(this IList<T> resources, ResourceLocation location) where T : ResourceRequiredContext
        {
            resources.ToList().ForEach(resource => resource.Settings.Location = location);
            return resources;
        }
    }
}