using Orchard;
using Orchard.ContentManagement;

namespace Coevery.Entities.Services {
    public interface IContentFieldValueProvider : IDependency {
        object GetValue(ContentItem contentItem, ContentField field);
    }
}