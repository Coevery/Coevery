using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries;

namespace Piedone.HelpfulLibraries.ServiceValidation
{
    [OrchardFeature("Piedone.HelpfulLibraries.ServiceValidation")]
    public class ServiceValidationModule : IModule
    {
        public void Configure(IComponentRegistry componentRegistry)
        {
            // This is necessary as generic dependencies are currently not resolved, see issue: http://orchard.codeplex.com/workitem/18141
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(ServiceValidationDictionary<>)).As(typeof(IServiceValidationDictionary<>));

            builder.Update(componentRegistry);
        }
    }
}
