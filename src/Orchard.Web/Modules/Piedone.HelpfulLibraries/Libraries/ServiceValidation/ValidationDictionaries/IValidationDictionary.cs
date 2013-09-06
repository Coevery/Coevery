using System.Collections.Generic;
using Orchard.Localization;

namespace Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries
{
    /// <summary>
    /// Stores validation data, similar to ModelStateDictionary in controllers
    /// </summary>
    /// <remarks>
    /// See also: http://www.asp.net/mvc/tutorials/validating-with-a-service-layer-cs
    /// </remarks>
    public interface IValidationDictionary
    {
        Dictionary<string, LocalizedString> Errors { get; }
        void AddError(string key, LocalizedString errorMessage);
        void AddError(string key, string errorMessage);
        bool IsValid { get; }
    }

    /// <summary>
    /// Stores validation data, similar to ModelStateDictionary in controllers
    /// </summary>
    /// <typeparam name="T">The type to use as the Errors dictionary key</typeparam>
    public interface IValidationDictionary<T>
    {
        Dictionary<T, LocalizedString> Errors { get; }
        void AddError(T key, LocalizedString errorMessage);
        void AddError(T key, string errorMessage);
        bool IsValid { get; }
    }
}
