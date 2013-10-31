using System.Xml.Linq;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.ContentManagement.MetaData {
    public interface IContentDefinitionWriter : IDependency{
        XElement Export(ContentTypeDefinition typeDefinition);
        XElement Export(ContentPartDefinition partDefinition);
    }
}
