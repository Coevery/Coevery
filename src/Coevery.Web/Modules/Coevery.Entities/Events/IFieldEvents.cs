using Coevery.Events;

namespace Coevery.Entities.Events {
    public interface IFieldEvents : IEventHandler {
        void OnCreated(string entityName, string fieldName, bool isInLayout);
        void OnDeleting(string entityName, string fieldName);
    }
}