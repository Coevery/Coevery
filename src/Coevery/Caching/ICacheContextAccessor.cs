namespace Coevery.Caching {
    public interface ICacheContextAccessor {
        IAcquireContext Current { get; set; }
    }
}