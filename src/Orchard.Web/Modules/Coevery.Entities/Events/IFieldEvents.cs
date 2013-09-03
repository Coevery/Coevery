using Orchard.Events;

namespace Coevery.Entities.Events {
    public interface IFieldEvents : IEventHandler {
        void OnDeleting(FieldEventsContext context);
    }

    public class FieldEventsContext {
        public string EtityName { get; set; }
        public string FieldName { get; set; }
        public bool IsCancel { get; set; }
    }
}
