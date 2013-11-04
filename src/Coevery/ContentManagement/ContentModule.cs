using Autofac;

namespace Coevery.ContentManagement {
    public class ContentModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<DefaultContentQuery>().As<IContentQuery>().InstancePerDependency();
        }
    }
}
