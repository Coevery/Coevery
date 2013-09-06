using Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries;

namespace Piedone.HelpfulLibraries.ServiceValidation.ServiceInterfaces
{
    /// <summary>
    /// Indicates a service that stores validation errors in an IServiceValidationDictionary<T>
    /// </summary>
    /// <typeparam name="T">The type to use as the IServiceValidationDictionary<T> type</typeparam>
    public interface IValidatingService<T>
    {
        IServiceValidationDictionary<T> ValidationDictionary { get; }
    }

    /// <summary>
    /// Indicates a service that stores validation errors in an IServiceValidationDictionary
    /// </summary>
    public interface IValidatingService: IValidatingService<string>
    {
    }
}
