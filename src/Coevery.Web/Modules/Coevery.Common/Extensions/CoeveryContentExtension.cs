using System.Collections.Generic;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.Common.Extensions {
    public class EntityNames {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string CollectionName { get; set; }
        public string CollectionDisplayName { get; set; }
    }

    public interface IContentDefinitionExtension:IDependency {
        IEnumerable<ContentTypeDefinition> ListUserDefinedTypeDefinitions();
        IEnumerable<ContentPartDefinition> ListUserDefinedPartDefinitions();
        EntityNames GetEntityNames(string entityName);
        string GetEntityNameFromCollectionName(string collectionname, bool isDisplayName=false);
    }
}
