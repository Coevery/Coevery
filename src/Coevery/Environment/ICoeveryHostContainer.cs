namespace Coevery.Environment {
    public interface ICoeveryHostContainer {
        T Resolve<T>();
    }
}