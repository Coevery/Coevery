using System.Collections.Generic;
using Coevery;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.Common.Extensions {
    public interface IContentDefinitionExtension:IDependency {
        IEnumerable<ContentTypeDefinition> ListUserDefinedTypeDefinitions();
        IEnumerable<ContentPartDefinition> ListUserDefinedPartDefinitions();
    }
}
