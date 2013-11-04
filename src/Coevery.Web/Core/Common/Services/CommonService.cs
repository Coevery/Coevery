using Coevery.ContentManagement;
using Coevery.Tasks.Scheduling;

namespace Coevery.Core.Common.Services {
    public class CommonService : ICommonService {
        private readonly IPublishingTaskManager _publishingTaskManager;
        private readonly IContentManager _contentManager;

        public CommonService(IPublishingTaskManager publishingTaskManager, IContentManager contentManager) {
            _publishingTaskManager = publishingTaskManager;
            _contentManager = contentManager;
        }

        void ICommonService.Publish(ContentItem contentItem) {
            _publishingTaskManager.DeleteTasks(contentItem);
            _contentManager.Publish(contentItem);
        }
    }
}