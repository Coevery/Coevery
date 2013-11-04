namespace Coevery.Caching {
    public interface IVolatileToken {
        bool IsCurrent { get; }
    }
}