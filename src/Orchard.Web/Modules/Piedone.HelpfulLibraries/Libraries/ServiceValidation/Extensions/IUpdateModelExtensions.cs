using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries;

namespace Piedone.HelpfulLibraries.ServiceValidation.Extensions
{
    [OrchardFeature("Piedone.HelpfulLibraries.ServiceValidation")]
    public static class IUpdateModelExtensions
    {
        /// <summary>
        /// Copies the error entries from an IValidationDictionary to an IUpdateModel for usage in a driver editor
        /// </summary>
        public static void TranscribeValidationDictionaryErrors(this IUpdateModel updater, IValidationDictionary validationDictionary)
        {
            foreach (var error in validationDictionary.Errors)
            {
                // Since default DataAnnotations messages are already localized in the .po files, it is no problem that
                // we are transcribing this way. Custom messages will be wrapped somewhere in the service inside a T(),
                // so the translation code generator will find them and generate their entries in the module's .po file.
                updater.AddModelError(error.Key, error.Value);
            }
        }

        /// <summary>
        /// Copies the error entries from an IValidationDictionary to an IUpdateModel for usage in a driver editor
        /// </summary>
        /// <typeparam name="T">The type parameter of the IValidationDictionary<T></typeparam>
        public static void TranscribeValidationDictionaryErrors<T>(this IUpdateModel updater, IValidationDictionary<T> validationDictionary)
        {
            foreach (var error in validationDictionary.Errors)
            {
                // Since default DataAnnotations messages are already localized in the .po files, it is no problem that
                // we are transcribing this way. Custom messages will be wrapped somewhere in the service inside a T(),
                // so the translation code generator will find them and generate their entries in the module's .po file.
                updater.AddModelError(error.Key.ToString(), error.Value);
            }
        }
    }
}