using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries
{
    [OrchardFeature("Piedone.HelpfulLibraries.ServiceValidation")]
    public class ServiceValidationDictionary : ValidationDictionary, IServiceValidationDictionary
    {
    }

    [OrchardFeature("Piedone.HelpfulLibraries.ServiceValidation")]
    public class ServiceValidationDictionary<T> : ValidationDictionary<T>, IServiceValidationDictionary<T>
    {
    }
}