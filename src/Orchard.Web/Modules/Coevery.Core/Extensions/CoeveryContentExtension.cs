using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement.MetaData.Models;

namespace Coevery.Core.Extensions {
    public interface IContentDefinitionExtension:IDependency {
        IEnumerable<ContentTypeDefinition> ListUserDefinedTypeDefinitions();
        IEnumerable<ContentPartDefinition> ListUserDefinedPartDefinitions();
    }
}
