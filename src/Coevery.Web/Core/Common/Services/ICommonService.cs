using Coevery.ContentManagement;

namespace Coevery.Core.Common.Services {
    public interface ICommonService : IDependency {
        void Publish(ContentItem contentItem);
    }
}