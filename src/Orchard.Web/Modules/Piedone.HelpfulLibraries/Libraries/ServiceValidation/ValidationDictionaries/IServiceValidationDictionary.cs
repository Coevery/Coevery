using Orchard;

namespace Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries
{
    /// <summary>
    /// Stores validation data in services
    /// </summary>
    public interface IServiceValidationDictionary : IValidationDictionary, ITransientDependency
    {
    }

    /// <summary>
    /// Stores validation data in services
    /// </summary>
    /// <typeparam name="T">The type to use as the Errors dictionary key</typeparam>
    public interface IServiceValidationDictionary<T> : IValidationDictionary<T>//, ITransientDependency
    {
    }
}
