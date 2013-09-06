using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.DependencyInjection
{
    [OrchardFeature("Piedone.HelpfulLibraries.DependencyInjection")]
    public class DependencyInjectionModule : IModule
    {
        public void Configure(IComponentRegistry componentRegistry)
        {
            // This is necessary as generic dependencies are currently not resolved, see issue: http://orchard.codeplex.com/workitem/18141
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(Resolve<>)).As(typeof(IResolve<>));

            builder.Update(componentRegistry);
        }
    }
}
