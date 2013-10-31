using System.Xml.Linq;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.ContentManagement.MetaData {
    public interface IContentDefinitionReader : IDependency {
        void Merge(XElement source, ContentTypeDefinitionBuilder builder);
        void Merge(XElement source, ContentPartDefinitionBuilder builder);
    }

    public static class ContentDefinitionReaderExtensions {
        public static ContentTypeDefinition Import(this IContentDefinitionReader reader, XElement source) {
            var target = new ContentTypeDefinitionBuilder();
            reader.Merge(source, target);
            return target.Build();
        }
    }
}