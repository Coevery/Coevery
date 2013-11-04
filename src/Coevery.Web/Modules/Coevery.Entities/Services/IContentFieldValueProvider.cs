using Coevery;
using Coevery.ContentManagement;

namespace Coevery.Entities.Services {
    public interface IContentFieldValueProvider : IDependency {
        object GetValue(ContentItem contentItem, ContentField field);
    }
}