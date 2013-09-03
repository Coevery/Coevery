using Orchard.Events;

namespace Coevery.Entities.Events {
    public interface IFieldEvents : IEventHandler {
        void OnCreated(FieldCreatedContext context);
        void OnDeleting(FieldDeletingContext context);
    }

    public class FieldCreatedContext {
        public string EtityName { get; set; }
        public string FieldName { get; set; }
        public bool IsInLayout { get; set; }
    }

    public class FieldDeletingContext {
        public string EtityName { get; set; }
        public string FieldName { get; set; }
        public bool IsCancel { get; set; }
    }
}
