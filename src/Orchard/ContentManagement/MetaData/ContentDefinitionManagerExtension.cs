using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.MetaData.Models;

namespace Orchard.ContentManagement.MetaData
{
    public static class ContentDefinitionServiceExtension
    {
        public static IEnumerable<ContentTypeDefinition> ListUserDefinedTypeDefinitions(this IContentDefinitionManager contentDefinitionManager) {
            var contentTypes = contentDefinitionManager.ListTypeDefinitions();
            var parts = contentDefinitionManager.ListPartDefinitions();
            return contentTypes.Where(t => parts.Any(p => p.Name == t.Name));
        }

        public static IEnumerable<ContentPartDefinition> ListUserDefinedPartDefinitions(this IContentDefinitionManager contentDefinitionManager)
        {
            var contentTypes = contentDefinitionManager.ListTypeDefinitions();
            var parts = contentDefinitionManager.ListPartDefinitions();
            return parts.Where(t => contentTypes.Any(p => p.Name == t.Name));
        }
    }
}