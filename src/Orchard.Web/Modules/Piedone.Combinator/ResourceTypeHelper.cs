using Orchard.Environment.Extensions;

namespace Piedone.Combinator
{
    [OrchardFeature("Piedone.Combinator")]
    public static class ResourceTypeHelper
    {
        public static ResourceType StringTypeToEnum(string resourceType)
        {
            if (resourceType == "stylesheet")
            {
                return ResourceType.Style;
            }

            return ResourceType.JavaScript;
        }

        public static string ToStringType(this ResourceType resourceType)
        {
            return EnumToStringType(resourceType);
        }

        public static string EnumToStringType(ResourceType resourceType)
        {
            if (resourceType == ResourceType.Style)
            {
                return "stylesheet";
            }

            return "script";
        }
    }
}