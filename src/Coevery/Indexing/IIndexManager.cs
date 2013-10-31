namespace Coevery.Indexing {
    public interface IIndexManager : IDependency {

        bool HasIndexProvider();
        IIndexProvider GetSearchIndexProvider();
    }
}