using Coevery.ContentManagement;
using Coevery.Events;

namespace Coevery.Entities.Events {
    public interface IEntityDataEvents : IEventHandler {
        void OnDeleting(DeletingEntityDataContext context);
    }

    public class DeletingEntityDataContext {
        public ContentItem ContentItem { get; set; }
        public bool IsCancel { get; set; }
        public string ErrorMessage { get; set; }
    }
}