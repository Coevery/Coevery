using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries;

namespace Piedone.HelpfulLibraries.ServiceValidation.Extensions
{
    [OrchardFeature("Piedone.HelpfulLibraries.ServiceValidation")]
    public static class ModelStateDictionaryExtensions
    {        
        /// <summary>
        /// Copies the error entries from an IValidationDictionary to a controller's ModelStateDictionary
        /// </summary>
        public static void TranscribeValidationDictionaryErrors(this ModelStateDictionary modelStateDictionary, IValidationDictionary validationDictionary)
        {
            foreach (var error in validationDictionary.Errors)
            {
                modelStateDictionary.AddModelError(error.Key, error.Value.ToString());
            }
        }

        /// <summary>
        /// Copies the error entries from an IValidationDictionary to a controller's ModelStateDictionary
        /// </summary>
        /// <typeparam name="T">The type parameter of the IValidationDictionary<T></typeparam>
        public static void TranscribeValidationDictionaryErrors<T>(this ModelStateDictionary modelStateDictionary, IValidationDictionary<T> validationDictionary)
        {
            foreach (var error in validationDictionary.Errors)
            {
                modelStateDictionary.AddModelError(error.Key.ToString(), error.Value.ToString());
            }
        }
    }
}