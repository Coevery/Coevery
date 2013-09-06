using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries
{
    [OrchardFeature("Piedone.HelpfulLibraries.ServiceValidation")]
    public class ValidationDictionary<T> : IValidationDictionary<T>
    {
        public ValidationDictionary()
        {
            Errors = new Dictionary<T, LocalizedString>();
        }

        #region IValidationDictionary Members
        public Dictionary<T, LocalizedString> Errors { get; private set; }

        public void AddError(T key, LocalizedString errorMessage)
        {
            Errors.Add(key, errorMessage);
        }

        public void AddError(T key, string errorMessage)
        {
            Errors.Add(key, new LocalizedString(errorMessage));
        }

        public bool IsValid
        {
            get
            {
                return Errors.Count == 0;
            }
        }
        #endregion
    }

    [OrchardFeature("Piedone.HelpfulLibraries.ServiceValidation")]
    public class ValidationDictionary : ValidationDictionary<string>
    {
    }
}