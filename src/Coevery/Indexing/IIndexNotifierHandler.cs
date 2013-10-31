using Coevery.Events;

namespace Coevery.Indexing {
    public interface IIndexNotifierHandler : IEventHandler {
        void UpdateIndex(string indexName);
    }
}
