using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.MetaData.Models;

namespace Orchard.ContentManagement.MetaData
{
    public static class ContentDefinitionServiceExtension
    {
        public static IEnumerable<ContentTypeDefinition> ListUserTypeDefinitions(this IContentDefinitionManager contentDefinitionManager)
        {
            var contentyTypes = contentDefinitionManager.ListTypeDefinitions();
            var typeNames = contentyTypes.Select(ctd => ctd.Name);
            var parts = contentDefinitionManager.ListPartDefinitions();
            var userParts = parts.Where(cpd => typeNames.Contains(cpd.Name));
            return contentyTypes.Where(c => userParts.Any(u => u.Name == c.Name));
        }
    }
}