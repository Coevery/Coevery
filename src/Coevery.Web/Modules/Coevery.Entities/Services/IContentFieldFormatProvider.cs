using Coevery.ContentManagement;

namespace Coevery.Entities.Services {
    public interface IContentFieldFormatProvider : IDependency {
        void SetFormat(ContentField field, dynamic formState);
    }
}