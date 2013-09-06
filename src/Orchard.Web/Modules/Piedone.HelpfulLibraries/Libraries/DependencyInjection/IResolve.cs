
namespace Piedone.HelpfulLibraries.DependencyInjection
{
    /// <summary>
    /// Helps lazy resolving of dependencies, without burying resolving with Resolve() calls
    /// </summary>
    /// <typeparam name="T">Type of the IDependency to resolve</typeparam>
    public interface IResolve<T>// : IDependency
    {
        /// <summary>
        /// The resolved instance
        /// </summary>
        T Value { get; }
    }
}
