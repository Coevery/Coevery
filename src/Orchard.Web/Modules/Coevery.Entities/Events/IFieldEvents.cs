using Orchard.Events;

namespace Coevery.Entities.Events {
    public interface IFieldEvents : IEventHandler {
        void OnCreated(string etityName, string fieldName, bool isInLayout);
        void OnDeleting(string etityName, string fieldName);
    }
}