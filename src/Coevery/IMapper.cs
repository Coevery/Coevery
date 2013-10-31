namespace Coevery {
    public interface IMapper<TSource, TTarget> : IDependency {
        TTarget Map(TSource source);
    }
}